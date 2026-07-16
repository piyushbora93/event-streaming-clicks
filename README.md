# FetchClicks

A sample implementation of one endpoint from a real-time retail ad-analytics platform:

```
GET /ad/{campaignId}/clicks  ->  number of customers who clicked the ad for a campaign
```

It is a slice of a larger streaming platform design. The focus here is on **code
structure, design principles and idiomatic use of ASP.NET Core** rather than on a
running database — the two data stores are represented with in-memory sample data so
the API runs end to end without external infrastructure.

## Where this fits in the platform

In the full design, events flow through Kafka, are aggregated by a stream
processor (Flink), and land in two storages:

- a **real-time store** (Redis) that holds the running/aggregate total for active campaigns
- a **historical store** (a data warehouse such as Snowflake) that holds the full historical data.

This endpoint reads from those two stores. The rule is simple:

1. Check the real-time store first (fast path for active campaigns).
2. If the campaign is not there, fall back to the historical store.
3. If neither has it, return `404`.

Because the real-time store keeps the *complete* running total while a campaign is
active, a single store always has the full answer so a simple fallback is correct
and we never have to merge results from both.

## Project structure

```
Controllers/
    AdController.cs                 HTTP endpoint, input validation, status codes
Services/
    Interfaces/IClicksService.cs    service contract
    ClicksService.cs                the real-time-first / historical-fallback logic
Repositories/
    Interfaces/IClicksRepository.cs contract shared by both stores
    RealTimeClicksRepository.cs     real-time store (Redis) - sample data
    HistoricalClicksRepository.cs   historical store (warehouse) - sample data
Models/
    ClicksResponse.cs               response shape
Program.cs                          dependency injection wiring
```

The layering is deliberate: the controller only deals with HTTP, the service holds the
business logic, and each repository only knows how to read its own store. Swapping the
sample repositories for real Redis / Snowflake implementations is a change in
`Program.cs` only — nothing in the controller or service changes.

## Design patterns and principles

- **Repository pattern** - data access hidden behind `IClicksRepository`.
- **Service layer** - the fallback logic lives in `ClicksService`, not the controller.
- **Dependency injection** - dependencies are constructor-injected and wired in `Program.cs`.
- **Separation of concerns** - Controller / Service / Repository each have a single job.
- **Depend on abstractions** - the controller depends on `IClicksService`, not the concrete type.

## Running it

Requires the .NET 8 SDK.

```
cd FetchClicks
dotnet run
```

Then open the Swagger UI (URL shown in the console, e.g. `https://localhost:7219/swagger`)
and try the endpoint.

## Sample data

| campaignId            | Result                          |
|-----------------------|---------------------------------|
| `campaign-1`, `campaign-2` | 200 - served from the real-time store |
| `campaign-3`, `campaign-4` | 200 - served from the historical store |
| anything else         | 404 - not found                 |

## Notes / what would change for production

- The two repositories would use real clients (`StackExchange.Redis` for the real-time
  store, a Snowflake connector for the historical store) instead of in-memory data.
- Authentication/authorization (per-tenant access control) would sit in front of the API.
- The other two endpoints (`/impressions`, `/clickToBasket`) follow the same shape.

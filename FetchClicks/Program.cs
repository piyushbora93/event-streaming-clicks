using System.Reflection;
using FetchClicks.Repositories;
using FetchClicks.Services;
using FetchClicks.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serialize enums as readable strings ("RealTime") instead of numbers (0).
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Surface the XML doc comments (/// ...) in the Swagger UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Both repositories implement IClicksRepository, so they are registered as concrete
// types: the service needs both stores (real-time first, historical fallback), and
// mapping two implementations to the same interface would be confusing for the container.
builder.Services.AddScoped<RealTimeClicksRepository>();
builder.Services.AddScoped<HistoricalClicksRepository>();
builder.Services.AddScoped<IClicksService, ClicksService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

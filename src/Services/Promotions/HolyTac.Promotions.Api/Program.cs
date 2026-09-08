using System.Text.Json.Serialization;
using HolyTac.Promotions.Application;
using HolyTac.Promotions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

// Sin CORS aquí a propósito: el navegador solo le habla al Gateway (donde sí se configura),
// nunca directo a este microservicio.
builder.Services.AddPromotionsApplication();
builder.Services.AddPromotionsInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

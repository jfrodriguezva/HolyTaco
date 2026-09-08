using HolyTac.Orders.Application;
using HolyTac.Orders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Sin CORS aquí a propósito: el navegador solo le habla al Gateway (donde sí se configura),
// nunca directo a este microservicio.
builder.Services.AddOrdersApplication();
builder.Services.AddOrdersInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

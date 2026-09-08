using System.Text;
using System.Threading.RateLimiting;
using HolyTac.Gateway.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.SectionName));
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddControllers();

var jwtSection = builder.Configuration.GetSection($"{AuthOptions.SectionName}:Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("StaffAuth", jwtOptions =>
    {
        // No mapear "role" a la URI larga de ClaimTypes.Role: se deja tal cual para que
        // RouteClaimsRequirement en ocelot.json lo compare literalmente ("role": "Gerente").
        jwtOptions.MapInboundClaims = false;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SigningKey"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });
builder.Services.AddAuthorization();

// Rate limiting: límite global por IP contra el Gateway completo, más un límite estricto y
// separado sobre /api/auth/login para frenar intentos de fuerza bruta contra contraseñas.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0,
            }));

    options.AddFixedWindowLimiter("login", loginOptions =>
    {
        loginOptions.PermitLimit = 5;
        loginOptions.Window = TimeSpan.FromMinutes(1);
        loginOptions.QueueLimit = 0;
    });
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddOcelot(builder.Configuration).AddPolly();

var app = builder.Build();

// UseRouting()/UseEndpoints() explícitos a propósito: Ocelot es middleware terminal (nunca llama
// next()), así que si se deja que MapControllers() difiera su ejecución al final del pipeline
// (comportamiento automático de WebApplication cuando no hay UseRouting/UseEndpoints explícitos),
// Ocelot intercepta la petición antes de que el controller llegue a ejecutarse.
app.UseRouting();
app.UseCors("DefaultCors");
app.UseRateLimiter();
app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.UseOcelot();

app.Run();

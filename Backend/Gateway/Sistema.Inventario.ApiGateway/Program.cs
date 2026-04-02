using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog para logging
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

// Configuración de Health Checks para monitoreo de salud del API Gateway
builder.Services.AddHealthChecks();

// Configuración de CORS para permitir solicitudes desde el frontend
string[] origenesPermitidos = builder.Configuration.GetSection("Cors:origenesPermitidos").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PoliticaFrontend", politica =>
    {
        if (origenesPermitidos.Length > 0)
        {
            politica.WithOrigins(origenesPermitidos)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

// Configuración de Ocelot para el API Gateway
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger para Ocelot
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

app.UseCors("PoliticaFrontend");

app.UseHealthChecks("/health");

await app.UseOcelot();

app.Run();

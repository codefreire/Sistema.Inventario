using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

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

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
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

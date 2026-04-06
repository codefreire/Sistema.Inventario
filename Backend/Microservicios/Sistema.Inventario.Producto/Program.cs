using Serilog;

using Sistema.Inventario.Producto.API.Extensiones;
using Sistema.Inventario.Producto.API.Middlewares;

// Resolver la ruta del proyecto para configurar correctamente el ContentRootPath y WebRootPath para el almacenamiento de archivos imagen
static string ResolverRutaProyectoProducto()
{
    string[] rutasBase =
    [
        Directory.GetCurrentDirectory(),
        AppContext.BaseDirectory
    ];

    foreach (string rutaBase in rutasBase)
    {
        DirectoryInfo? directorio = new DirectoryInfo(rutaBase);
        while (directorio is not null)
        {
            string rutaCsproj = Path.Combine(directorio.FullName, "Sistema.Inventario.Producto.csproj");
            if (File.Exists(rutaCsproj))
            {
                return directorio.FullName;
            }

            directorio = directorio.Parent;
        }
    }

    return Directory.GetCurrentDirectory();
}
string rutaProyecto = ResolverRutaProyectoProducto();
var opcionesAplicacion = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = rutaProyecto,
    WebRootPath = Path.Combine(rutaProyecto, "wwwroot")
};

var builder = WebApplication.CreateBuilder(opcionesAplicacion);

// Configuración de Serilog
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddProductos(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseHealthChecks("/health");

app.UseManejoExcepciones();

app.UseTiempoRequest();

app.MapControllers();

app.Run();
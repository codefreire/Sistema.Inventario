using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sistema.Inventario.Storage.DTOs;

namespace Sistema.Inventario.Storage.Servicios;

/// <summary>
/// Servicio de almacenamiento en disco local para imágenes.
/// </summary>
public class AlmacenamientoServicio : IAlmacenamientoServicio
{
    private readonly IWebHostEnvironment _entorno;
    private readonly IConfiguration _configuracion;
    private readonly ILogger<AlmacenamientoServicio> _registrador;

    private static readonly string[] _extensionesPermitidas = new string[] { ".jpg", ".jpeg", ".png", ".webp" };
    private const long TamanoMaximoBytes = 5L * 1024 * 1024;
    private const string CarpetaUploads = "uploads";

    /// <summary>
    /// Constructor del servicio de almacenamiento.
    /// </summary>
    /// <param name="entorno">Entorno web del host.</param>
    /// <param name="configuracion">Configuración del host consumidor.</param>
    /// <param name="registrador">Registrador de eventos.</param>
    public AlmacenamientoServicio(IWebHostEnvironment entorno, IConfiguration configuracion, ILogger<AlmacenamientoServicio> registrador)
    {
        _entorno = entorno;
        _configuracion = configuracion;
        _registrador = registrador;
    }

    /// <inheritdoc/>
    public async Task<ArchivoSubidoResponse> GuardarArchivoAsync(SubirArchivoRequest solicitud)
    {
        if (solicitud.Archivo is null || solicitud.Archivo.Length == 0)
        {
            throw new ArgumentException("El archivo no puede estar vacío.");
        }

        if (solicitud.Archivo.Length > TamanoMaximoBytes)
        {
            throw new ArgumentException("El archivo supera el tamaño máximo permitido de 5 MB.");
        }

        string extension = Path.GetExtension(solicitud.Archivo.FileName).ToLowerInvariant();
        if (!_extensionesPermitidas.Contains(extension))
        {
            throw new ArgumentException("La extensión del archivo no está permitida. Use jpg, jpeg, png o webp.");
        }

        string nombreArchivo = $"{Guid.NewGuid()}{extension}";
        string rutaUploads = ObtenerRutaUploads();
        string rutaArchivo = Path.Combine(rutaUploads, nombreArchivo);

        if (!Directory.Exists(rutaUploads))
        {
            Directory.CreateDirectory(rutaUploads);
        }

        await using (FileStream flujoArchivo = new FileStream(rutaArchivo, FileMode.Create))
        {
            await solicitud.Archivo.CopyToAsync(flujoArchivo);
        }

        string urlBase = _configuracion["Almacenamiento:UrlBase"]
            ?? throw new InvalidOperationException("La configuración 'Almacenamiento:UrlBase' es obligatoria.");
        string urlPublica = $"{urlBase.TrimEnd('/')}/{CarpetaUploads}/{nombreArchivo}";

        _registrador.LogInformation("Archivo almacenado correctamente: {NombreArchivo}", nombreArchivo);

        return new ArchivoSubidoResponse
        {
            NombreArchivo = nombreArchivo,
            UrlPublica = urlPublica,
            TipoContenido = solicitud.Archivo.ContentType,
            TamanoBytes = solicitud.Archivo.Length
        };
    }

    /// <inheritdoc/>
    public Task<bool> EliminarArchivoAsync(string nombreArchivo)
    {
        string nombreSeguro = Path.GetFileName(nombreArchivo);
        string rutaArchivo = Path.Combine(ObtenerRutaUploads(), nombreSeguro);

        if (!File.Exists(rutaArchivo))
        {
            _registrador.LogWarning("No se encontró el archivo para eliminar: {NombreArchivo}", nombreSeguro);
            return Task.FromResult(false);
        }

        File.Delete(rutaArchivo);
        _registrador.LogInformation("Archivo eliminado correctamente: {NombreArchivo}", nombreSeguro);
        return Task.FromResult(true);
    }

    private string ObtenerRutaUploads()
    {
        string webRootPath = _entorno.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        return Path.Combine(webRootPath, CarpetaUploads);
    }
}

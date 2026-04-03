using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sistema.Inventario.Storage.Dominio;
using Sistema.Inventario.Storage.DTOs.Requests;
using Sistema.Inventario.Storage.DTOs.Responses;

namespace Sistema.Inventario.Storage.Servicios;

/// <summary>
/// Servicio de almacenamiento para imágenes
/// </summary>
public class AlmacenamientoServicio : IAlmacenamientoServicio
{
    /// <summary>
    /// Entorno web del host
    /// </summary>
    private readonly IWebHostEnvironment _entorno;

    /// <summary>
    /// Configuración para obtener la URL base de acceso a las imágenes
    /// </summary>
    private readonly IConfiguration _configuracion;

    /// <summary>
    /// Servicio de logging para mostrar eventos de almacenamiento y eliminación de archivos imagen
    /// </summary>
    private readonly ILogger<AlmacenamientoServicio> _logger;

    /// <summary>
    /// Constructor del servicio de almacenamiento
    /// </summary>
    /// <param name="entorno">Entorno web del host</param>
    /// <param name="configuracion">Configuración del host consumidor</param>
    /// <param name="logger">Servicio de logging para mostrar eventos</param>
    public AlmacenamientoServicio(IWebHostEnvironment entorno, IConfiguration configuracion, ILogger<AlmacenamientoServicio> logger)
    {
        _entorno = entorno;
        _configuracion = configuracion;
        _logger = logger;
    }

    /// <summary>
    /// Guarda un archivo de imagen en el almacenamiento
    /// </summary>
    /// <param name="request">Solicitud con el archivo imagen a guardar</param>
    /// <returns>Datos del archivo imagen guardado</returns>
    public async Task<ArchivoImagenResponse> GuardarArchivoAsync(ArchivoImagenRequest request)
    {
        if (request.Archivo is null || request.Archivo.Length == 0)
        {
            throw new ArgumentException("El archivo no puede estar vacío.");
        }

        if (request.Archivo.Length > Constantes.TamanioMaximoBytes)
        {
            throw new ArgumentException("El archivo supera el tamaño máximo permitido es de 5 MB.");
        }

        string extension = Path.GetExtension(request.Archivo.FileName).ToLowerInvariant();
        if (extension != Constantes.ExtensionJpg && extension != Constantes.ExtensionJpeg && extension != Constantes.ExtensionPng && extension != Constantes.ExtensionWebp)
        {
            throw new ArgumentException("La extensión del archivo no está permitida. Extensiones permitidas: jpg, jpeg, png o webp.");
        }

        string nombreArchivoImagen = $"{Guid.NewGuid()}{extension}";
        string rutaImagenes = ObtenerRutaImagenes();
        string rutaArchivoImagen = Path.Combine(rutaImagenes, nombreArchivoImagen);

        if (!Directory.Exists(rutaImagenes))
        {
            Directory.CreateDirectory(rutaImagenes);
        }

        await using (FileStream flujoArchivo = new FileStream(rutaArchivoImagen, FileMode.Create))
        {
            await request.Archivo.CopyToAsync(flujoArchivo);
        }

        string urlBase = _configuracion["Almacenamiento:UrlBase"]
            ?? throw new InvalidOperationException("La configuración 'Almacenamiento:UrlBase' es obligatoria.");
        string urlImagen = $"{urlBase.TrimEnd('/')}/{Constantes.CarpetaImagenes}/{nombreArchivoImagen}";

        _logger.LogInformation("Archivo almacenado correctamente: {NombreArchivo}", nombreArchivoImagen);

        return new ArchivoImagenResponse
        {
            NombreArchivo = nombreArchivoImagen,
            UrlImagen = urlImagen,
            TipoContenido = request.Archivo.ContentType,
            TamanioBytes = request.Archivo.Length
        };
    }

    /// <summary>
    /// Elimina un archivo de imagen del almacenamiento por su nombre único
    /// </summary>
    /// <param name="nombreArchivo">Nombre único del archivo imagen</param>
    /// <returns>True si fue eliminado o false si no se encontró</returns>
    public Task<bool> EliminarArchivoAsync(string nombreArchivo)
    {
        string nombreImagen = Path.GetFileName(nombreArchivo);
        string rutaArchivo = Path.Combine(ObtenerRutaImagenes(), nombreImagen);

        if (!File.Exists(rutaArchivo))
        {
            _logger.LogWarning("No se encontró el archivo para eliminar: {NombreArchivo}", nombreImagen);
            return Task.FromResult(false);
        }

        File.Delete(rutaArchivo);
        _logger.LogInformation("Archivo eliminado correctamente: {NombreArchivo}", nombreImagen);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Obtiene la ruta de la carpeta donde se almacenan los archivos imagen, creando la carpeta si no existe
    /// </summary>
    /// <returns>Ruta de la carpeta de imágenes</returns>
    private string ObtenerRutaImagenes()
    {
        string webRootPath = _entorno.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        return Path.Combine(webRootPath, Constantes.CarpetaImagenes);
    }
}

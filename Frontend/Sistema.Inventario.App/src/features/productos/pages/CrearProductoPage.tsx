import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { productoService } from '../services/productoService';
import { validarProducto, tieneErrores } from '../validators/productoValidator';
import type { ErroresProducto } from '../validators/productoValidator';
import type { CrearProductoRequest } from '../types/Producto';
import Button from '../../../shared/components/Button';
import Notificacion from '../../../shared/components/Notificacion';

/**
 * Renderiza el formulario para registrar un nuevo producto en el inventario.
 */
export default function CrearProductoPage() {
  const navigate = useNavigate();
  const [modoImagen, setModoImagen] = useState<'url' | 'archivo'>('url');
  const [archivoSeleccionado, setArchivoSeleccionado] = useState<File | null>(null);
  const [previewArchivo, setPreviewArchivo] = useState<string>('');
  const [subiendoImagen, setSubiendoImagen] = useState(false);
  const [formulario, setFormulario] = useState<CrearProductoRequest>({
    nombre: '',
    descripcion: '',
    categoria: '',
    imagenUrl: '',
    precio: 0,
    stock: 0,
  });
  const [errores, setErrores] = useState<ErroresProducto>({});
  const [enviando, setEnviando] = useState(false);
  const [notificacion, setNotificacion] = useState({ mensaje: '', tipo: 'exito' as 'exito' | 'error', visible: false });

  const handleCambio = (campo: keyof CrearProductoRequest, valor: string | number) => {
    setFormulario((prev) => ({ ...prev, [campo]: valor }));
    setErrores((prev) => ({ ...prev, [campo]: undefined }));
  };

  /**
   * Maneja el archivo de imagen seleccionado en modo "archivo" y genera preview local.
   */
  const handleArchivoImagen = (archivo: File | null) => {
    if (!archivo) {
      setArchivoSeleccionado(null);
      setPreviewArchivo('');
      return;
    }

    setArchivoSeleccionado(archivo);
    setPreviewArchivo(URL.createObjectURL(archivo));
    setErrores((prev) => ({ ...prev, imagenUrl: undefined }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      setEnviando(true);
      let imagenUrlFinal = formulario.imagenUrl;

      if (modoImagen === 'archivo') {
        if (!archivoSeleccionado) {
          setErrores((prev) => ({ ...prev, imagenUrl: 'Debe seleccionar un archivo de imagen.' }));
          return;
        }

        setSubiendoImagen(true);
        imagenUrlFinal = await productoService.subirImagen(archivoSeleccionado);
      }

      const payload: CrearProductoRequest = {
        ...formulario,
        imagenUrl: imagenUrlFinal,
      };

      const erroresValidacion = validarProducto(payload);
      setErrores(erroresValidacion);

      if (tieneErrores(erroresValidacion)) return;

      await productoService.crear(payload);
      setNotificacion({ mensaje: 'Producto creado correctamente.', tipo: 'exito', visible: true });
      setTimeout(() => navigate('/productos'), 1500);
    } catch (error) {
      const mensaje = error instanceof Error ? error.message : 'Error al crear el producto.';
      setNotificacion({ mensaje, tipo: 'error', visible: true });
    } finally {
      setSubiendoImagen(false);
      setEnviando(false);
    }
  };

  return (
    <div className="pagina">
      <Notificacion
        mensaje={notificacion.mensaje}
        tipo={notificacion.tipo}
        visible={notificacion.visible}
        onCerrar={() => setNotificacion((n) => ({ ...n, visible: false }))}
      />

      <div className="pagina-encabezado">
        <h1>Crear Producto</h1>
      </div>

      <form className="formulario" onSubmit={handleSubmit}>
        <div className="campo">
          <label htmlFor="nombre">Nombre *</label>
          <input id="nombre" type="text" maxLength={50} value={formulario.nombre} onChange={(e) => handleCambio('nombre', e.target.value)} />
          {errores.nombre && <span className="campo-error">{errores.nombre}</span>}
        </div>

        <div className="campo">
          <label htmlFor="descripcion">Descripción *</label>
          <textarea id="descripcion" maxLength={500} rows={3} value={formulario.descripcion} onChange={(e) => handleCambio('descripcion', e.target.value)} />
          {errores.descripcion && <span className="campo-error">{errores.descripcion}</span>}
        </div>

        <div className="campo">
          <label htmlFor="categoria">Categoría *</label>
          <input id="categoria" type="text" maxLength={50} value={formulario.categoria} onChange={(e) => handleCambio('categoria', e.target.value)} />
          {errores.categoria && <span className="campo-error">{errores.categoria}</span>}
        </div>

        <div className="campo">
          <label>Origen de Imagen *</label>
          <div className="selector-imagen">
            <Button
              type="button"
              variante={modoImagen === 'url' ? 'primario' : 'secundario'}
              onClick={() => {
                setModoImagen('url');
                setArchivoSeleccionado(null);
                setPreviewArchivo('');
                setErrores((prev) => ({ ...prev, imagenUrl: undefined }));
              }}
            >
              Usar URL
            </Button>
            <Button
              type="button"
              variante={modoImagen === 'archivo' ? 'primario' : 'secundario'}
              onClick={() => {
                setModoImagen('archivo');
                setFormulario((prev) => ({ ...prev, imagenUrl: '' }));
                setErrores((prev) => ({ ...prev, imagenUrl: undefined }));
              }}
            >
              Subir Archivo
            </Button>
          </div>

          {modoImagen === 'url' ? (
            <input
              id="imagenUrl"
              type="url"
              maxLength={500}
              placeholder="https://dominio.com/imagen.jpg"
              value={formulario.imagenUrl}
              onChange={(e) => handleCambio('imagenUrl', e.target.value)}
            />
          ) : (
            <input
              id="archivoImagen"
              type="file"
              accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp"
              onChange={(e) => handleArchivoImagen(e.target.files?.[0] ?? null)}
            />
          )}

          {previewArchivo && modoImagen === 'archivo' && (
            <div className="preview-imagen-contenedor">
              <img className="preview-imagen" src={previewArchivo} alt="Vista previa del archivo de producto" />
            </div>
          )}

          {errores.imagenUrl && <span className="campo-error">{errores.imagenUrl}</span>}
          {modoImagen === 'archivo' && <span className="campo-info">Primero se sube la imagen y luego se usa su URL final en imagenUrl.</span>}
        </div>

        <div className="formulario-fila">
          <div className="campo">
            <label htmlFor="precio">Precio ($) *</label>
            <input id="precio" type="number" min="0" step="0.01" max="99999999.99" value={formulario.precio} onChange={(e) => handleCambio('precio', parseFloat(e.target.value) || 0)} />
            {errores.precio && <span className="campo-error">{errores.precio}</span>}
          </div>

          <div className="campo">
            <label htmlFor="stock">Stock *</label>
            <input id="stock" type="number" min="0" step="1" value={formulario.stock} onChange={(e) => handleCambio('stock', parseInt(e.target.value) || 0)} />
            {errores.stock && <span className="campo-error">{errores.stock}</span>}
          </div>
        </div>

        <div className="formulario-acciones">
          <Button variante="secundario" type="button" onClick={() => navigate('/productos')}>Cancelar</Button>
          <Button type="submit" disabled={enviando || subiendoImagen}>{enviando || subiendoImagen ? 'Guardando...' : 'Crear Producto'}</Button>
        </div>
      </form>
    </div>
  );
}

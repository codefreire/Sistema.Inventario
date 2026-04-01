import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { productoService } from '../services/productoService';
import { validarProducto, tieneErrores } from '../validators/productoValidator';
import type { ErroresProducto } from '../validators/productoValidator';
import type { ActualizarProductoRequest } from '../types/Producto';
import Button from '../../../shared/components/Button';
import Notificacion from '../../../shared/components/Notificacion';

export default function EditarProductoPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [formulario, setFormulario] = useState<ActualizarProductoRequest>({
    nombre: '',
    descripcion: '',
    categoria: '',
    imagenUrl: '',
    precio: 0,
    stock: 0,
  });
  const [errores, setErrores] = useState<ErroresProducto>({});
  const [enviando, setEnviando] = useState(false);
  const [cargando, setCargando] = useState(true);
  const [notificacion, setNotificacion] = useState({ mensaje: '', tipo: 'exito' as 'exito' | 'error', visible: false });

  useEffect(() => {
    if (!id) return;
    productoService
      .obtenerPorId(id)
      .then((producto) => {
        setFormulario({
          nombre: producto.nombre,
          descripcion: producto.descripcion,
          categoria: producto.categoria,
          imagenUrl: producto.imagenUrl,
          precio: producto.precio,
          stock: producto.stock,
        });
      })
      .catch((error) => {
        const mensaje = error instanceof Error ? error.message : 'Error al cargar el producto.';
        setNotificacion({ mensaje, tipo: 'error', visible: true });
      })
      .finally(() => setCargando(false));
  }, [id]);

  const handleCambio = (campo: keyof ActualizarProductoRequest, valor: string | number) => {
    setFormulario((prev) => ({ ...prev, [campo]: valor }));
    setErrores((prev) => ({ ...prev, [campo]: undefined }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const erroresValidacion = validarProducto(formulario);
    setErrores(erroresValidacion);

    if (tieneErrores(erroresValidacion)) return;

    try {
      setEnviando(true);
      await productoService.actualizar(id!, formulario);
      setNotificacion({ mensaje: 'Producto actualizado correctamente.', tipo: 'exito', visible: true });
      setTimeout(() => navigate('/productos'), 1500);
    } catch (error) {
      const mensaje = error instanceof Error ? error.message : 'Error al actualizar el producto.';
      setNotificacion({ mensaje, tipo: 'error', visible: true });
    } finally {
      setEnviando(false);
    }
  };

  if (cargando) return <div className="pagina"><p>Cargando producto...</p></div>;

  return (
    <div className="pagina">
      <Notificacion
        mensaje={notificacion.mensaje}
        tipo={notificacion.tipo}
        visible={notificacion.visible}
        onCerrar={() => setNotificacion((n) => ({ ...n, visible: false }))}
      />

      <div className="pagina-encabezado">
        <h1>Editar Producto</h1>
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
          <label htmlFor="imagenUrl">URL de Imagen *</label>
          <input id="imagenUrl" type="url" maxLength={500} value={formulario.imagenUrl} onChange={(e) => handleCambio('imagenUrl', e.target.value)} />
          {errores.imagenUrl && <span className="campo-error">{errores.imagenUrl}</span>}
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
          <Button type="submit" disabled={enviando}>{enviando ? 'Guardando...' : 'Actualizar Producto'}</Button>
        </div>
      </form>
    </div>
  );
}

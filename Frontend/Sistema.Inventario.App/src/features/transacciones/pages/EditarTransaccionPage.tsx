import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { transaccionService } from '../services/transaccionService';
import { productoService } from '../../productos/services/productoService';
import { validarTransaccion, tieneErrores } from '../validators/transaccionValidator';
import type { ErroresTransaccion } from '../validators/transaccionValidator';
import type { ActualizarTransaccionRequest } from '../types/Transaccion';
import type { Producto } from '../../productos/types/Producto';
import Button from '../../../shared/components/Button';
import Notificacion from '../../../shared/components/Notificacion';

/**
 * Renderiza el formulario para editar una transacción existente.
 */
export default function EditarTransaccionPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [productos, setProductos] = useState<Producto[]>([]);
  const [formulario, setFormulario] = useState<ActualizarTransaccionRequest>({
    tipoTransaccion: '',
    productoId: '',
    cantidad: 1,
    precioUnitario: 0,
    detalle: '',
  });
  const [errores, setErrores] = useState<ErroresTransaccion>({});
  const [enviando, setEnviando] = useState(false);
  const [cargando, setCargando] = useState(true);
  const [notificacion, setNotificacion] = useState({ mensaje: '', tipo: 'exito' as 'exito' | 'error', visible: false });

  useEffect(() => {
    const cargarDatos = async () => {
      try {
        const [transaccion, listaProductos] = await Promise.all([
          transaccionService.obtenerPorId(id!),
          productoService.obtenerTodos(),
        ]);
        setProductos(listaProductos);
        setFormulario({
          tipoTransaccion: transaccion.tipoTransaccion,
          productoId: transaccion.productoId,
          cantidad: transaccion.cantidad,
          precioUnitario: transaccion.precioUnitario,
          detalle: transaccion.detalle,
        });
      } catch (error) {
        const mensaje = error instanceof Error ? error.message : 'Error al cargar la transacción.';
        setNotificacion({ mensaje, tipo: 'error', visible: true });
      } finally {
        setCargando(false);
      }
    };
    if (id) cargarDatos();
  }, [id]);

  const productoSeleccionado = productos.find((p) => p.id === formulario.productoId);

  const handleCambio = (campo: keyof ActualizarTransaccionRequest, valor: string | number) => {
    setFormulario((prev) => ({ ...prev, [campo]: valor }));
    setErrores((prev) => ({ ...prev, [campo]: undefined }));
  };

  const handleSeleccionarProducto = (productoId: string) => {
    const producto = productos.find((p) => p.id === productoId);
    setFormulario((prev) => ({
      ...prev,
      productoId,
      precioUnitario: producto ? producto.precio : 0,
    }));
    setErrores((prev) => ({ ...prev, productoId: undefined }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const stockDisponible = productoSeleccionado?.stock;
    const erroresValidacion = validarTransaccion(formulario, stockDisponible);
    setErrores(erroresValidacion);

    if (tieneErrores(erroresValidacion)) return;

    try {
      setEnviando(true);
      await transaccionService.actualizar(id!, formulario);
      setNotificacion({ mensaje: 'Transacción actualizada correctamente.', tipo: 'exito', visible: true });
      setTimeout(() => navigate('/transacciones'), 1500);
    } catch (error) {
      const mensaje = error instanceof Error ? error.message : 'Error al actualizar la transacción.';
      setNotificacion({ mensaje, tipo: 'error', visible: true });
    } finally {
      setEnviando(false);
    }
  };

  if (cargando) return <div className="pagina"><p>Cargando transacción...</p></div>;

  return (
    <div className="pagina">
      <Notificacion
        mensaje={notificacion.mensaje}
        tipo={notificacion.tipo}
        visible={notificacion.visible}
        onCerrar={() => setNotificacion((n) => ({ ...n, visible: false }))}
      />

      <div className="pagina-encabezado">
        <h1>Editar Transacción</h1>
      </div>

      <form className="formulario" onSubmit={handleSubmit}>
        <div className="campo">
          <label htmlFor="tipoTransaccion">Tipo de Transacción *</label>
          <select id="tipoTransaccion" value={formulario.tipoTransaccion} onChange={(e) => handleCambio('tipoTransaccion', e.target.value)}>
            <option value="">Seleccione un tipo</option>
            <option value="Compra">Compra</option>
            <option value="Venta">Venta</option>
          </select>
          {errores.tipoTransaccion && <span className="campo-error">{errores.tipoTransaccion}</span>}
        </div>

        <div className="campo">
          <label htmlFor="productoId">Producto *</label>
          <select id="productoId" value={formulario.productoId} onChange={(e) => handleSeleccionarProducto(e.target.value)}>
            <option value="">Seleccione un producto</option>
            {productos.map((p) => (
              <option key={p.id} value={p.id}>
                {p.nombre} (Stock: {p.stock})
              </option>
            ))}
          </select>
          {errores.productoId && <span className="campo-error">{errores.productoId}</span>}
          {productoSeleccionado && (
            <span className="campo-info">
              Stock disponible: {productoSeleccionado.stock} unidades
            </span>
          )}
        </div>

        <div className="formulario-fila">
          <div className="campo">
            <label htmlFor="cantidad">Cantidad *</label>
            <input id="cantidad" type="number" min="1" step="1" value={formulario.cantidad} onChange={(e) => handleCambio('cantidad', parseInt(e.target.value) || 0)} />
            {errores.cantidad && <span className="campo-error">{errores.cantidad}</span>}
          </div>

          <div className="campo">
            <label htmlFor="precioUnitario">Precio Unitario ($) *</label>
            <input id="precioUnitario" type="number" min="0" step="0.01" max="99999999.99" value={formulario.precioUnitario} onChange={(e) => handleCambio('precioUnitario', parseFloat(e.target.value) || 0)} />
            {errores.precioUnitario && <span className="campo-error">{errores.precioUnitario}</span>}
          </div>
        </div>

        <div className="campo">
          <label htmlFor="detalle">Detalle *</label>
          <textarea id="detalle" maxLength={500} rows={3} value={formulario.detalle} onChange={(e) => handleCambio('detalle', e.target.value)} />
          {errores.detalle && <span className="campo-error">{errores.detalle}</span>}
        </div>

        <div className="formulario-acciones">
          <Button variante="secundario" type="button" onClick={() => navigate('/transacciones')}>Cancelar</Button>
          <Button type="submit" disabled={enviando}>{enviando ? 'Guardando...' : 'Actualizar Transacción'}</Button>
        </div>
      </form>
    </div>
  );
}

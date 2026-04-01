import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { productoService } from '../services/productoService';
import type { Producto } from '../types/Producto';
import { formatearMoneda } from '../../../shared/utils/formatCurrency';
import Button from '../../../shared/components/Button';
import Notificacion from '../../../shared/components/Notificacion';

const IMAGEN_FALLBACK = "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='96' height='96' viewBox='0 0 96 96'%3E%3Crect width='96' height='96' fill='%23edf4ef'/%3E%3Cpath d='M20 66l16-18 12 12 9-9 19 15' stroke='%2300693c' stroke-width='4' fill='none'/%3E%3Ccircle cx='33' cy='31' r='6' fill='%2387cb53'/%3E%3C/svg%3E";

export default function ConsultarProductoPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [producto, setProducto] = useState<Producto | null>(null);
  const [cargando, setCargando] = useState(true);
  const [notificacion, setNotificacion] = useState({ mensaje: '', tipo: 'exito' as 'exito' | 'error', visible: false });

  useEffect(() => {
    if (!id) {
      setNotificacion({ mensaje: 'No se recibió el identificador del producto.', tipo: 'error', visible: true });
      setCargando(false);
      return;
    }

    productoService
      .obtenerPorId(id)
      .then((respuesta) => setProducto(respuesta))
      .catch((error) => {
        const mensaje = error instanceof Error ? error.message : 'Error al consultar el producto.';
        setNotificacion({ mensaje, tipo: 'error', visible: true });
      })
      .finally(() => setCargando(false));
  }, [id]);

  if (cargando) {
    return (
      <div className="pagina">
        <p>Cargando información del producto...</p>
      </div>
    );
  }

  if (!producto) {
    return (
      <div className="pagina">
        <Notificacion
          mensaje={notificacion.mensaje}
          tipo={notificacion.tipo}
          visible={notificacion.visible}
          onCerrar={() => setNotificacion((n) => ({ ...n, visible: false }))}
        />

        <div className="pagina-encabezado">
          <h1>Consulta de Producto</h1>
        </div>

        <Button variante="secundario" type="button" onClick={() => navigate('/productos')}>
          Volver a Productos
        </Button>
      </div>
    );
  }

  return (
    <div className="pagina">
      <Notificacion
        mensaje={notificacion.mensaje}
        tipo={notificacion.tipo}
        visible={notificacion.visible}
        onCerrar={() => setNotificacion((n) => ({ ...n, visible: false }))}
      />

      <div className="pagina-encabezado">
        <h1>Consulta de Producto</h1>
      </div>

      <div className="formulario">
        <div className="campo">
          <label>Imagen</label>
          <div className="preview-imagen-contenedor">
            <img
              className="preview-imagen"
              src={producto.imagenUrl || IMAGEN_FALLBACK}
              alt={`Imagen de ${producto.nombre}`}
              onError={(e) => {
                e.currentTarget.src = IMAGEN_FALLBACK;
              }}
            />
          </div>
        </div>

        <div className="campo">
          <label htmlFor="nombre">Nombre</label>
          <input id="nombre" type="text" value={producto.nombre} readOnly />
        </div>

        <div className="campo">
          <label htmlFor="descripcion">Descripción</label>
          <textarea id="descripcion" rows={3} value={producto.descripcion} readOnly />
        </div>

        <div className="campo">
          <label htmlFor="categoria">Categoría</label>
          <input id="categoria" type="text" value={producto.categoria} readOnly />
        </div>

        <div className="campo">
          <label htmlFor="imagenUrl">URL de Imagen</label>
          <input id="imagenUrl" type="text" value={producto.imagenUrl} readOnly />
        </div>

        <div className="formulario-fila">
          <div className="campo">
            <label htmlFor="precio">Precio</label>
            <input id="precio" type="text" value={formatearMoneda(producto.precio)} readOnly />
          </div>

          <div className="campo">
            <label htmlFor="stock">Stock</label>
            <input id="stock" type="text" value={String(producto.stock)} readOnly />
          </div>
        </div>

        <div className="formulario-acciones">
          <Button variante="secundario" type="button" onClick={() => navigate('/productos')}>
            Volver a Productos
          </Button>
          <Button type="button" onClick={() => navigate(`/productos/editar/${producto.id}`)}>
            Ir a Edición
          </Button>
        </div>
      </div>
    </div>
  );
}

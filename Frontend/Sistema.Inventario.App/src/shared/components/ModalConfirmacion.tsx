import Button from './Button';

interface ModalConfirmacionProps {
  visible: boolean;
  titulo: string;
  mensaje: string;
  onConfirmar: () => void;
  onCancelar: () => void;
}

/**
 * Modal de confirmacion para acciones potencialmente destructivas o sensibles.
 *
 * Comportamiento:
 * - Si `visible` es `false`, no renderiza contenido.
 * - Cierra al hacer clic en el fondo (`onCancelar`).
 * - Evita el cierre al hacer clic dentro del contenido del modal.
 * - Expone acciones explicitas de cancelar y confirmar.
 *
 * @param {ModalConfirmacionProps} props Propiedades del componente.
 * @param {boolean} props.visible Controla si el modal se muestra.
 * @param {string} props.titulo Titulo principal del cuadro de dialogo.
 * @param {string} props.mensaje Mensaje descriptivo de la confirmacion.
 * @param {() => void} props.onConfirmar Callback al confirmar la accion.
 * @param {() => void} props.onCancelar Callback al cancelar o cerrar el modal.
 * @returns {JSX.Element | null} Estructura del modal cuando esta visible, o `null` cuando no lo esta.
 *
 * @example
 * <ModalConfirmacion
 *   visible={mostrarModal}
 *   titulo="Eliminar producto"
 *   mensaje="Esta accion no se puede deshacer."
 *   onConfirmar={eliminarProducto}
 *   onCancelar={() => setMostrarModal(false)}
 * />
 */
export default function ModalConfirmacion({
  visible,
  titulo,
  mensaje,
  onConfirmar,
  onCancelar,
}: ModalConfirmacionProps) {
  if (!visible) return null;

  return (
    <div className="modal-fondo" onClick={onCancelar}>
      <div className="modal-contenido" onClick={(e) => e.stopPropagation()}>
        <h3>{titulo}</h3>
        <p>{mensaje}</p>
        <div className="modal-acciones">
          <Button variante="secundario" onClick={onCancelar}>Cancelar</Button>
          <Button variante="peligro" onClick={onConfirmar}>Confirmar</Button>
        </div>
      </div>
    </div>
  );
}

import Button from './Button';

interface ModalConfirmacionProps {
  visible: boolean;
  titulo: string;
  mensaje: string;
  onConfirmar: () => void;
  onCancelar: () => void;
}

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

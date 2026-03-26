import { useEffect } from 'react';

export interface NotificacionProps {
  mensaje: string;
  tipo: 'exito' | 'error' | 'info';
  visible: boolean;
  onCerrar: () => void;
}

export default function Notificacion({ mensaje, tipo, visible, onCerrar }: NotificacionProps) {
  useEffect(() => {
    if (visible) {
      const timer = setTimeout(onCerrar, 4000);
      return () => clearTimeout(timer);
    }
  }, [visible, onCerrar]);

  if (!visible) return null;

  const iconos = { exito: '✅', error: '❌', info: 'ℹ️' };

  return (
    <div className={`notificacion notificacion-${tipo}`}>
      <span>{iconos[tipo]} {mensaje}</span>
      <button className="notificacion-cerrar" onClick={onCerrar}>×</button>
    </div>
  );
}

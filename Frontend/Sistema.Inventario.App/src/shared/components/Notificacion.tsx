import { useEffect } from 'react';

export interface NotificacionProps {
  mensaje: string;
  tipo: 'exito' | 'error' | 'info';
  visible: boolean;
  onCerrar: () => void;
}

/**
 * Componente de notificacion tipo toast para mensajes de exito, error o informacion.
 *
 * Comportamiento de auto-cierre:
 * - Cuando `visible` es `true`, inicia un temporizador de 4 segundos.
 * - Al cumplirse el tiempo, ejecuta `onCerrar` automaticamente.
 * - Si el componente se oculta o desmonta antes, limpia el temporizador.
 *
 * @param {NotificacionProps} props Propiedades de la notificacion.
 * @param {string} props.mensaje Texto a mostrar al usuario.
 * @param {'exito' | 'error' | 'info'} props.tipo Variante visual y de icono.
 * @param {boolean} props.visible Define si la notificacion se renderiza.
 * @param {() => void} props.onCerrar Callback para cerrar manual o automaticamente.
 * @returns {JSX.Element | null} Estructura de la notificacion visible, o `null` si esta oculta.
 *
 * @example
 * <Notificacion
 *   mensaje="Producto guardado correctamente"
 *   tipo="exito"
 *   visible={mostrarToast}
 *   onCerrar={() => setMostrarToast(false)}
 * />
 */
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

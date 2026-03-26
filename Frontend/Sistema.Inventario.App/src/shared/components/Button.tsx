import type { ButtonHTMLAttributes } from 'react';

interface BotonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variante?: 'primario' | 'secundario' | 'peligro' | 'exito';
}

/**
 * Boton reutilizable del sistema con variantes visuales predefinidas.
 *
 * Variantes disponibles en `variante`:
 * - primario: accion principal de la pantalla.
 * - secundario: accion alternativa o de bajo enfasis.
 * - peligro: acciones destructivas (ej. eliminar).
 * - exito: acciones positivas o confirmatorias.
 *
 * @param {BotonProps} props Propiedades del boton.
 * @param {'primario' | 'secundario' | 'peligro' | 'exito'} [props.variante='primario'] Estilo visual del boton.
 * @returns {JSX.Element} Elemento `button` con clases CSS segun la variante.
 *
 * @example
 * <Button variante="peligro" onClick={onEliminar}>Eliminar</Button>
 */
export default function Button({ variante = 'primario', className = '', children, ...props }: BotonProps) {
  return (
    <button className={`btn btn-${variante} ${className}`} {...props}>
      {children}
    </button>
  );
}
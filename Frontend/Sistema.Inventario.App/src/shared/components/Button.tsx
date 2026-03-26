import type { ButtonHTMLAttributes } from 'react';

interface BotonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variante?: 'primario' | 'secundario' | 'peligro' | 'exito';
}

export default function Button({ variante = 'primario', className = '', children, ...props }: BotonProps) {
  return (
    <button className={`btn btn-${variante} ${className}`} {...props}>
      {children}
    </button>
  );
}
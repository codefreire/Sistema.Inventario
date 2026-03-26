/**
 * Formatea un número como moneda en dólares estadounidenses
 * Utiliza la configuración regional de Ecuador (es-EC)
 * @param {number} valor - Valor numérico a formatear
 * @returns {string} Valor formateado como moneda (ej: $1,234.56)
 * @example
 * formatearMoneda(1234.567) // Retorna: "$1,234.57"
 */
export function formatearMoneda(valor: number): string {
  return new Intl.NumberFormat('es-EC', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 2,
  }).format(valor);
}
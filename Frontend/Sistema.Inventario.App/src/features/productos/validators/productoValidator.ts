import type { CrearProductoRequest } from '../types/Producto';

export interface ErroresProducto {
  nombre?: string;
  descripcion?: string;
  categoria?: string;
  imagenUrl?: string;
  precio?: string;
  stock?: string;
}

export function validarProducto(producto: CrearProductoRequest): ErroresProducto {
  const errores: ErroresProducto = {};

  if (!producto.nombre || producto.nombre.trim() === '') {
    errores.nombre = 'El nombre es obligatorio.';
  } else if (producto.nombre.length > 50) {
    errores.nombre = 'El nombre no puede exceder 50 caracteres.';
  }

  if (!producto.descripcion || producto.descripcion.trim() === '') {
    errores.descripcion = 'La descripción es obligatoria.';
  } else if (producto.descripcion.length > 500) {
    errores.descripcion = 'La descripción no puede exceder 500 caracteres.';
  }

  if (!producto.categoria || producto.categoria.trim() === '') {
    errores.categoria = 'La categoría es obligatoria.';
  } else if (producto.categoria.length > 50) {
    errores.categoria = 'La categoría no puede exceder 50 caracteres.';
  }

  if (!producto.imagenUrl || producto.imagenUrl.trim() === '') {
    errores.imagenUrl = 'La URL de la imagen es obligatoria.';
  } else {
    try {
      new URL(producto.imagenUrl);
    } catch {
      errores.imagenUrl = 'La URL de la imagen no es válida.';
    }
  }

  if (producto.precio === undefined || producto.precio === null) {
    errores.precio = 'El precio es obligatorio.';
  } else if (producto.precio < 0) {
    errores.precio = 'El precio debe ser mayor o igual a 0.';
  } else if (producto.precio > 99999999.99) {
    errores.precio = 'El precio no puede exceder 99,999,999.99.';
  }

  if (producto.stock === undefined || producto.stock === null) {
    errores.stock = 'El stock es obligatorio.';
  } else if (!Number.isInteger(producto.stock)) {
    errores.stock = 'El stock debe ser un número entero.';
  } else if (producto.stock < 0) {
    errores.stock = 'El stock debe ser mayor o igual a 0.';
  }

  return errores;
}

export function tieneErrores(errores: ErroresProducto): boolean {
  return Object.values(errores).some((error) => error !== undefined);
}
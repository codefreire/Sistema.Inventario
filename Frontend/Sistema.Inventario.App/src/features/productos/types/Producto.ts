export interface Producto {
  id: string;
  nombre: string;
  descripcion: string;
  categoria: string;
  imagenUrl: string;
  precio: number;
  stock: number;
}

export interface CrearProductoRequest {
  nombre: string;
  descripcion: string;
  categoria: string;
  imagenUrl: string;
  precio: number;
  stock: number;
}

export interface ActualizarProductoRequest {
  nombre: string;
  descripcion: string;
  categoria: string;
  imagenUrl: string;
  precio: number;
  stock: number;
}

export interface SubirImagenResponse {
  imagenUrl: string;
}

export interface FiltrosProducto {
  nombre?: string;
  categoria?: string;
  precioMin?: number;
  precioMax?: number;
  stockMin?: number;
  stockMax?: number;
}
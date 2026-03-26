import { Routes, Route, Navigate } from 'react-router-dom';
import ProductosPage from '../features/productos/pages/ProductosPage';
import CrearProductoPage from '../features/productos/pages/CrearProductoPage';
import EditarProductoPage from '../features/productos/pages/EditarProductoPage';
import TransaccionesPage from '../features/transacciones/pages/TransaccionesPage';
import CrearTransaccionPage from '../features/transacciones/pages/CrearTransaccionPage';
import EditarTransaccionPage from '../features/transacciones/pages/EditarTransaccionPage';

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/productos" replace />} />
      <Route path="/productos" element={<ProductosPage />} />
      <Route path="/productos/crear" element={<CrearProductoPage />} />
      <Route path="/productos/editar/:id" element={<EditarProductoPage />} />
      <Route path="/transacciones" element={<TransaccionesPage />} />
      <Route path="/transacciones/crear" element={<CrearTransaccionPage />} />
      <Route path="/transacciones/editar/:id" element={<EditarTransaccionPage />} />
    </Routes>
  );
}
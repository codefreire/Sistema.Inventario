import { NavLink } from 'react-router-dom';

/**
 * Renderiza la barra de navegación principal con accesos a productos y transacciones.
 */
export default function Navbar() {
  return (
    <nav className="navbar">
      <div className="navbar-marca">
        <NavLink to="/">📦 Sistema Inventario</NavLink>
      </div>
      <ul className="navbar-enlaces">
        <li>
          <NavLink to="/productos" className={({ isActive }) => (isActive ? 'activo' : '')}>
            Productos
          </NavLink>
        </li>
        <li>
          <NavLink to="/transacciones" className={({ isActive }) => (isActive ? 'activo' : '')}>
            Transacciones
          </NavLink>
        </li>
      </ul>
    </nav>
  );
}
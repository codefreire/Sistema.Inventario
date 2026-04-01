# Sistema de Inventario - Produbanco

Aplicación web para la gestión de inventarios construida con una arquitectura de microservicios. Backend en **.NET 10** y Frontend en **React 19** con TypeScript.

---

## Requisitos

### Software necesario

| Herramienta | Versión mínima |
|-------------|---------------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0 |
| [Node.js](https://nodejs.org/) | 18.x o superior |
| [SQL Server](https://www.microsoft.com/sql-server) | 2019 o superior |
| [Git](https://git-scm.com/) | 2.x |

### Puertos utilizados

| Servicio | Puerto |
|----------|--------|
| API Gateway | `http://localhost:7000` |
| Microservicio de Productos | `http://localhost:5261` |
| Microservicio de Transacciones | `http://localhost:5253` |
| Frontend (Vite) | `http://localhost:5173` |

---

## Base de Datos

Ejecutar el script SQL ubicado en la raíz del proyecto para crear las bases de datos y tablas necesarias:

```sql
-- Abrir SQL Server Management Studio o Azure Data Studio
-- Ejecutar el archivo: script-base-datos.sql
```

Este script crea:
- **InventarioProductosBD** → Tabla `Productos` (Id, Nombre, Descripcion, Categoria, ImagenUrl, Precio, Stock)
- **InventarioTransaccionesBD** → Tabla `Transacciones` (Id, Fecha, TipoTransaccion, ProductoId, Cantidad, PrecioUnitario, PrecioTotal, Detalle)

> **Nota:** Verificar que el connection string en los archivos `appsettings.json` de cada microservicio apunte a su instancia de SQL Server. Por defecto usan `Server=LAPTOP-2MO9QJ8P` con autenticación integrada de Windows.

---

## Ejecución del Backend

Se deben ejecutar los **3 proyectos** simultáneamente (cada uno en una terminal separada):

### 1. Microservicio de Productos

```bash
cd Backend/Microservicios/Sistema.Inventario.Producto
dotnet run
```

Se ejecuta en `http://localhost:5261`. Swagger disponible en `http://localhost:5261/swagger`.

### 2. Microservicio de Transacciones

```bash
cd Backend/Microservicios/Sistema.Inventario.Transaccion
dotnet run
```

Se ejecuta en `http://localhost:5253`. Swagger disponible en `http://localhost:5253/swagger`.

### 3. API Gateway (Ocelot)

```bash
cd Backend/Gateway/Sistema.Inventario.ApiGateway
dotnet run
```

Se ejecuta en `http://localhost:7000`. Swagger unificado disponible en `http://localhost:7000/swagger`.

El Gateway enruta las peticiones:
- `http://localhost:7000/api/productos/*` → Microservicio de Productos
- `http://localhost:7000/api/transacciones/*` → Microservicio de Transacciones

### Ejecución de Tests del Backend

```bash
cd Backend/Tests/Sistema.Inventario.Producto.Tests
dotnet test

cd Backend/Tests/Sistema.Inventario.Transaccion.Tests
dotnet test
```

---

## Ejecución del Frontend

```bash
cd Frontend/Sistema.Inventario.App
npm install
npm run dev
```

Se ejecuta en `http://localhost:5173`.

El frontend se comunica con el API Gateway (`http://localhost:7000/api`) configurado en el archivo `.env`.

### Ejecución de Tests del Frontend

```bash
cd Frontend/Sistema.Inventario.App
npm test
```

Para generar reporte de cobertura HTML:

```bash
npm run test:coverage
```

El reporte se genera en `Frontend/Sistema.Inventario.App/coverage/index.html`.

---

## Arquitectura

```
┌─────────────┐     ┌──────────────────┐     ┌─────────────────────┐
│   Frontend   │────▶│   API Gateway    │────▶│  Micro. Productos   │
│  React + TS  │     │  Ocelot :7000    │     │    .NET :5261       │
│   :5173      │     │                  │     │  InventarioProductos│
└─────────────┘     │                  │     └─────────────────────┘
                     │                  │              ▲
                     │                  │              │ HTTP (síncrono)
                     │                  │              │
                     │                  │     ┌─────────────────────┐
                     │                  │────▶│ Micro. Transacciones│
                     │                  │     │    .NET :5253       │
                     └──────────────────┘     │ InventarioTransacc. │
                                              └─────────────────────┘
```

- **Comunicación síncrona:** El microservicio de Transacciones se comunica con el microservicio de Productos vía HTTP para validar stock y ajustarlo después de cada transacción.

---

## Evidencias

> **Nota:** Agregar capturas de pantalla de la aplicación en funcionamiento a continuación.

### Listado dinámico de productos con paginación
<!-- ![Listado Productos](evidencias/listado-productos.png) -->

### Listado dinámico de transacciones con paginación
<!-- ![Listado Transacciones](evidencias/listado-transacciones.png) -->

### Pantalla para la creación de productos
<!-- ![Crear Producto](evidencias/crear-producto.png) -->

### Pantalla para la edición de productos
<!-- ![Editar Producto](evidencias/editar-producto.png) -->

### Pantalla para la creación de transacciones
<!-- ![Crear Transacción](evidencias/crear-transaccion.png) -->

### Pantalla para la edición de transacciones
<!-- ![Editar Transacción](evidencias/editar-transaccion.png) -->

### Pantalla de filtros dinámicos
<!-- ![Filtros Dinámicos](evidencias/filtros-dinamicos.png) -->

### Consulta de información de un producto
<!-- ![Consulta Producto](evidencias/consulta-producto.png) -->

# Sistema de Inventario - Produbanco

Aplicación web para la gestión de inventarios construida con arquitectura de microservicios.

- Backend: .NET 10 + ASP.NET Core + Ocelot + SQL Server
- Frontend: React + TypeScript + Vite
- Comunicación entre microservicios: HTTP síncrono

---

## Requisitos

### Software necesario

| Herramienta | Versión mínima recomendada |
|-------------|----------------------------|
| .NET SDK | 10.0 |
| Node.js | 18.x |
| npm | 9.x |
| SQL Server (MSSQLSERVER) | 2019 o superior |
| SQL Server Management Studio (SSMS) o Azure Data Studio | Última |
| Git | 2.x |

### Puertos de la solución

| Componente | URL |
|------------|-----|
| API Gateway | http://localhost:7000 |
| Microservicio Productos | http://localhost:5261 |
| Microservicio Transacciones | http://localhost:5253 |
| Frontend (Vite) | http://localhost:5173 |

### Variables de entorno Frontend

El frontend consume el gateway vía `VITE_API_BASE_URL`.

Archivo de referencia: `Frontend/Sistema.Inventario.App/.env.example`

Ejemplo en `Frontend/Sistema.Inventario.App/.env`:

```env
VITE_API_BASE_URL=http://localhost:7000/api
```

### Dependencias clave (no base de plantilla)

| Dependencia | Ubicación | Uso principal |
|-------------|-----------|---------------|
| `Ocelot` | Backend/Gateway | Enrutamiento del API Gateway hacia microservicios. |
| `MMLib.SwaggerForOcelot` | Backend/Gateway | Swagger unificado del Gateway. |
| `Serilog.AspNetCore` + `Serilog.Settings.Configuration` + `Serilog.Sinks.File` | Backend (Gateway + Microservicios) | Logging estructurado en `logs/log-.txt`. |
| `FluentValidation.AspNetCore` | Backend (Productos/Transacciones) | Validación de requests en capa Aplicación. |
| `Microsoft.EntityFrameworkCore.SqlServer` | Backend (Productos/Transacciones) | Persistencia en SQL Server. |
| `Swashbuckle.AspNetCore.*` | Backend (Productos/Transacciones) | Documentación Swagger/OpenAPI por microservicio. |
| `axios` | Frontend | Cliente HTTP para consumo de APIs (`apiClient`). |
| `react-router-dom` | Frontend | Navegación y ruteo entre vistas. |
| `vitest` | Frontend/Tests | Runner de pruebas frontend. |
| `@testing-library/react` + `@testing-library/jest-dom` + `@testing-library/user-event` | Frontend/Tests | Pruebas de componentes e integración. |
| `@vitest/coverage-v8` | Frontend/Tests | Reporte de cobertura frontend. |
| `msw` | Frontend/Tests | Mocks de red en pruebas frontend. |
| `xUnit` + `Moq` + `WebApplicationFactory` | Backend/Tests | Pruebas unitarias e integración en microservicios. |

---

## Arquitectura Final

```text
Frontend (React + TS + Vite :5173)
    |
    v
API Gateway (Ocelot :7000)
     |-------------------------------> Microservicio Productos (.NET :5261)
     |                                 - Clean Architecture
     |                                 - CRUD Productos + carga de imágenes
     |                                 - DB: InventarioProductosBD
     |
     |-------------------------------> Microservicio Transacciones (.NET :5253)
                                                  - Clean Architecture
                                                  - CRUD Transacciones + reglas de stock
                                                  - Cliente HTTP síncrono a Productos
                                                  - DB: InventarioTransaccionesBD
```

Notas de arquitectura:

1. El microservicio de Transacciones consulta y ajusta stock en Productos de forma síncrona.
2. El módulo `Backend/Storage/Sistema.Inventario.Storage` centraliza la lógica de almacenamiento de imágenes.
3. El frontend consume únicamente el gateway (`/api/productos` y `/api/transacciones`).

### Módulos del proyecto

1. `Backend/Gateway/Sistema.Inventario.ApiGateway`
    - Puerta de entrada única del frontend.
    - Enrutamiento Ocelot a microservicios.
    - CORS configurado para `http://localhost:5173`.
    - Health check (`/health`) y Swagger unificado.
    - Observabilidad con Serilog a `logs/log-.txt`.

2. `Backend/Storage/Sistema.Inventario.Storage`
    - Librería compartida para almacenamiento de archivos.
    - Validación de extensión/tamaño de imagen.
    - Escritura en `wwwroot/uploads` y retorno de URL pública.

3. `Backend/Microservicios/Sistema.Inventario.Producto`
    - Arquitectura limpia por capas: API, Aplicación, Dominio, Infraestructura.
    - CRUD de productos, ajuste de stock y endpoint multipart para imágenes.
    - Static files habilitados para servir imágenes subidas.
    - FluentValidation, health check, middleware de tiempo de request.
    - Logging con Serilog en `logs/log-.txt`.

4. `Backend/Microservicios/Sistema.Inventario.Transaccion`
    - Arquitectura limpia por capas: API, Aplicación, Dominio, Infraestructura.
    - CRUD de transacciones con reglas de negocio de compra/venta.
    - Validación de stock y ajuste de stock en Productos vía HttpClient síncrono.
    - FluentValidation, health check, middleware de tiempo de request.
    - Logging con Serilog en `logs/log-.txt`.

5. `Backend/Tests`
    - `Sistema.Inventario.Producto.Tests` y `Sistema.Inventario.Transaccion.Tests`.
    - Pruebas unitarias + integración (xUnit/Moq/WebApplicationFactory).
    - Convención AAA explícita (Arrange, Act, Assert).
    - Se incluye reporte de cobertura en backend de Productos.

6. `Frontend/Sistema.Inventario.App`
    - Feature-Based Architecture (`features/productos`, `features/transacciones`).
    - Ruteo centralizado y componentes compartidos en `shared`.
    - Pruebas unitarias e integración con Vitest + Testing Library.
    - Cobertura HTML en `coverage/index.html`.

### Principios y patrones usados

1. Backend por microservicios con separación de responsabilidades.
2. Clean Architecture dentro de cada microservicio (API, Aplicación, Dominio, Infraestructura).
3. Comunicación síncrona entre servicios por HTTP REST.
4. Validaciones de entrada con FluentValidation.
5. Observabilidad con Serilog (archivo `logs/log-.txt`) y health checks.
6. Testing con patrón AAA en pruebas unitarias e integración.
7. Frontend con Feature-Based Architecture para escalabilidad y mantenimiento.
8. Documentación de código:
    - Backend con comentarios XML (`///`) en clases, métodos y contratos.
    - Frontend con comentarios JSDoc/TypeScript en servicios, utilidades y componentes clave.
9. Documentación de APIs con Swagger en Productos, Transacciones y Gateway.
10. Validaciones y manejo de excepciones:
    - Backend: FluentValidation en capa Aplicación, validación de tipos/propiedades y reglas de negocio.
    - Backend: Middleware de captura de excepciones globales registrando con Serilog.
    - Frontend: Validación de formularios con feedback al usuario vía componentes de notificación.
    - Frontend: Manejo de errores HTTP y timouts con mensajes claros en interfaz.

---

## Creación de Base de Datos en MSSQLSERVER

El script de creación está en la raíz del repositorio:

- `script.sql`

Pasos sugeridos (SSMS):

1. Abrir SQL Server Management Studio.
2. Conectarse a la instancia `MSSQLSERVER` (o la instancia local que uses).
3. Abrir el archivo `script.sql`.
4. Ejecutar el script completo.

El script crea (si no existen):

1. Base `InventarioProductosBD` con tabla `Productos` e índice `IX_Productos_Nombre`.
2. Base `InventarioTransaccionesBD` con tabla `Transacciones` e índices `IX_Transacciones_ProductoId` y `IX_Transacciones_Fecha`.

Importante:

1. El script es idempotente para tablas e índices (puede ejecutarse varias veces sin duplicar objetos).
2. Revisa/ajusta los connection strings de:
   - `Backend/Microservicios/Sistema.Inventario.Producto/appsettings.json`
   - `Backend/Microservicios/Sistema.Inventario.Transaccion/appsettings.json`

---

## Ejecución del Backend

Ejecutar en terminales separadas, en este orden:

### 1. Microservicio de Productos

```bash
cd Backend/Microservicios/Sistema.Inventario.Producto
dotnet run
```

Endpoints útiles:

- API: `http://localhost:5261/api/productos`
- Swagger: `http://localhost:5261/swagger`

### 2. Microservicio de Transacciones

```bash
cd Backend/Microservicios/Sistema.Inventario.Transaccion
dotnet run
```

Endpoints útiles:

- API: `http://localhost:5253/api/transacciones`
- Swagger: `http://localhost:5253/swagger`

### 3. API Gateway

```bash
cd Backend/Gateway/Sistema.Inventario.ApiGateway
dotnet run
```

Endpoints útiles:

- Gateway: `http://localhost:7000`
- Swagger Gateway: `http://localhost:7000/swagger`

Rutas principales a través del gateway:

1. `http://localhost:7000/api/productos/*`
2. `http://localhost:7000/api/transacciones/*`

---

## Ejecución del Frontend

```bash
cd Frontend/Sistema.Inventario.App
npm install
npm run dev
```

Aplicación disponible en:

- `http://localhost:5173`

---

## Ejecución de Pruebas

### Backend

```bash
cd Backend/Tests/Sistema.Inventario.Producto.Tests
dotnet test

cd ../Sistema.Inventario.Transaccion.Tests
dotnet test
```

### Frontend

```bash
cd Frontend/Sistema.Inventario.App
npm test
```

Cobertura frontend:

```bash
npm run test:coverage
```

Reporte generado en:

- `Frontend/Sistema.Inventario.App/coverage/index.html`

---

## Evidencias

Agregar las capturas en esta sección (esquema solicitado por la evaluación):

### 1. Listado dinámico de productos con paginación
<!-- ![Listado dinámico de productos](evidencias/listado-productos-paginacion.png) -->

### 2. Listado dinámico de transacciones con paginación
<!-- ![Listado dinámico de transacciones](evidencias/listado-transacciones-paginacion.png) -->

### 3. Pantalla para la creación de productos
<!-- ![Creación de productos](evidencias/crear-producto.png) -->

### 4. Pantalla para la edición de productos
<!-- ![Edición de productos](evidencias/editar-producto.png) -->

### 5. Pantalla para la creación de transacciones
<!-- ![Creación de transacciones](evidencias/crear-transaccion.png) -->

### 6. Pantalla para la edición de transacciones
<!-- ![Edición de transacciones](evidencias/editar-transaccion.png) -->

### 7. Pantalla de filtros dinámicos
<!-- ![Filtros dinámicos](evidencias/filtros-dinamicos.png) -->

### 8. Pantalla para la consulta de información de un formulario (extra)
<!-- ![Consulta de información de formulario](evidencias/consulta-formulario-extra.png) -->

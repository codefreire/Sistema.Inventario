# Sistema de Inventario

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
| Visual Studio Code | Última |
| SQL Server (MSSQLSERVER) | 2019 o superior |
| SQL Server Management Studio (SSMS) o Azure Data Studio | Última |
| Git | 2.x |

**Recomendación:** Levantar y ejecutar esta solución desde Visual Studio Code para trabajar los microservicios, el gateway, el frontend y las pruebas en terminales separadas dentro del mismo workspace.

### Puertos de la solución

| Componente | URL |
|------------|-----|
| API Gateway | http://localhost:7000 |
| Microservicio Productos | http://localhost:5261 |
| Microservicio Transacciones | http://localhost:5253 |
| Frontend (Vite) | http://localhost:5173 |

### Variables de entorno Frontend

El frontend consume el gateway vía `VITE_API_BASE_URL`.

**Pasos de configuración:**

1. Copiar el archivo `Frontend/Sistema.Inventario.App/.env.example`
2. Renombrarlo a `.env` en el mismo directorio
3. Colocar la URL del API Gateway:

```env
VITE_API_BASE_URL=http://localhost:7000/api
```

**Nota:** El archivo `.env` será ignorado por Git (ver `.gitignore`). El `.env.example` se usa como referencia para otros desarrolladores.

### Dependencias clave

| Dependencia | Ubicación | Uso principal |
|-------------|-----------|---------------|
| `Ocelot` | Backend/Gateway | Enrutamiento del API Gateway hacia microservicios. |
| `MMLib.SwaggerForOcelot` | Backend/Gateway | Swagger unificado del Gateway. |
| `Serilog.AspNetCore` + `Serilog.Settings.Configuration` + `Serilog.Sinks.File` | Backend (Gateway + Microservicios) | Logging estructurado en `logs/log-.txt`. |
| `FluentValidation.AspNetCore` | Backend (Productos/Transacciones) | Validación de requests en capa Aplicación. |
| `Microsoft.Extensions.Http.Polly` + `Polly` + `Polly.Extensions.Http` | Backend (Transacciones) | Resiliencia HTTP en llamadas a Productos (`Timeout`, `Retry`, `Circuit Breaker`, `Fallback`). |
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

<p align="center">
    <img src="ImagenesEvidencias/arquitectura.png" alt="Arquitectura Final" width="980" />
</p>

Notas de arquitectura:

1. El microservicio de Transacciones consulta y ajusta stock en Productos de forma síncrona.
2. El módulo `Backend/Storage/Sistema.Inventario.Storage` centraliza la lógica de almacenamiento de imágenes.
3. El frontend consume únicamente el gateway (`/api/productos` y `/api/transacciones`).

### Módulos del proyecto

1. `Backend/Gateway/Sistema.Inventario.ApiGateway`
    - Plantilla: `web`.
    - Puerta de entrada única del frontend.
    - Enrutamiento Ocelot a microservicios.
    - CORS configurado para `http://localhost:5173`.
    - Health check (`/health`) y Swagger unificado.
    - Telemetría con Serilog a `logs/log-.txt`.

2. `Backend/Storage/Sistema.Inventario.Storage`
    - Plantilla: `classlib`.
    - Librería para almacenamiento de archivos.
    - Validación de extensión/tamaño de imagen.
    - Escritura en `wwwroot/imagenes` y retorno de URL pública.

3. `Backend/Microservicios/Sistema.Inventario.Producto`
    - Plantilla: `webapi`.
    - Arquitectura limpia por capas: API, Aplicación, Dominio, Infraestructura.
    - CRUD de productos, ajuste de stock y endpoint multipart para imágenes.
    - Static files habilitados para servir imágenes subidas.
    - FluentValidation, health check.
    - `ManejoExcepcionesMiddleware`: (Middleware Personalizado) intercepta `ArgumentException` en el pipeline y responde con `400 Bad Request` con el mensaje de error, evitando que lleguen excepciones no manejadas al cliente.
    - `TiempoRequestMiddleware`: (Middleware Personalizado) mide con `Stopwatch` la duración de cada request y registra `HTTP {método} {ruta} => {status} en {ms} ms` vía Serilog.
    - Logging con Serilog en `logs/log-.txt`.

4. `Backend/Microservicios/Sistema.Inventario.Transaccion`
    - Plantilla: `webapi`.
    - Arquitectura limpia por capas: API, Aplicación, Dominio, Infraestructura.
    - CRUD de transacciones con reglas de negocio de compra/venta.
    - Validación de stock y ajuste de stock en Productos vía HttpClient síncrono.
    - Patrones de resiliencia en llamadas HTTP a Productos: `Timeout` por request, `Retry` con backoff exponencial (incluye 429), `Circuit Breaker`, `Fallback` controlado en lecturas (`GET`) y logging estructurado de eventos de resiliencia.
    - FluentValidation, health check.
    - `ManejoExcepcionesMiddleware`: (Middleware Personalizado) intercepta `ArgumentException` en el pipeline y responde con `400 Bad Request` con el mensaje de error, evitando que lleguen excepciones no manejadas al cliente.
    - `TiempoRequestMiddleware`: (Middleware Personalizado) mide con `Stopwatch` la duración de cada request y registra `HTTP {método} {ruta} => {status} en {ms} ms` vía Serilog.
    - Logging con Serilog en `logs/log-.txt`.

5. `Backend/Tests/Sistema.Inventario.Producto.Tests` y `Backend/Tests/Sistema.Inventario.Transaccion.Tests`
    - Plantilla: `xunit`.
    - `Sistema.Inventario.Producto.Tests` y `Sistema.Inventario.Transaccion.Tests`.
    - Pruebas unitarias + integración (xUnit/Moq/WebApplicationFactory).
    - Convención AAA explícita (Arrange, Act, Assert).
    - Reporte de cobertura en backend de Productos.

6. `Frontend/Sistema.Inventario.App`
    - Feature-Based Architecture (`features/productos`, `features/transacciones`).
    - Ruteo centralizado y componentes compartidos en `shared`.
    - Fallback de imágenes en vistas de productos para mostrar imagen de respaldo cuando la URL falla o no existe.
    - Pruebas unitarias e integración con Vitest + Testing Library.
    - Cobertura HTML en `coverage/index.html`.

### Principios, patrones y buenas prácticas

1. Backend por microservicios con separación de responsabilidades.
2. Clean Architecture dentro de cada microservicio (API, Aplicación, Dominio, Infraestructura).
3. Comunicación síncrona entre servicios por HTTP REST.
4. Validaciones de entrada con FluentValidation.
5. Telemetría con Serilog (archivo `logs/log-.txt`) y health checks.
6. Testing con patrón AAA en pruebas unitarias e integración.
7. Frontend con Feature-Based Architecture para escalabilidad y mantenimiento.
8. Documentación de código:
    - Backend con comentarios XML (`///`) en clases, métodos y contratos.
    - Frontend con comentarios JSDoc/TypeScript en servicios, utilidades y componentes clave.
9. Documentación de APIs con Swagger en Productos, Transacciones y Gateway.
10. Validaciones y manejo de excepciones:
    - Backend: FluentValidation en capa Aplicación, validación de tipos/propiedades y reglas de negocio.
    - Backend: `ManejoExcepcionesMiddleware` — captura `ArgumentException` lanzadas por la capa de dominio/almacenamiento, registra con Serilog (`LogWarning`) y responde `400 Bad Request` con el mensaje de error como texto plano.
    - Backend: `TiempoRequestMiddleware` — registra con Serilog (`LogInformation`) el método HTTP, la ruta, el status code y la duración en milisegundos de cada request.
    - Frontend: Validación de formularios con feedback al usuario vía componentes de notificación.
    - Frontend: Manejo de errores HTTP y timouts con mensajes claros en interfaz.

### Resiliencia en Transacciones

El microservicio de Transacciones aplica resiliencia en su comunicación HTTP con Productos para mejorar estabilidad ante fallos temporales:

1. `Timeout` por request de `10` segundos en `HttpClient` tipado.
2. `Retry` con backoff exponencial (`2s`, `4s`, `8s`) para errores transitorios, `timeout` y `429 TooManyRequests`.
3. `Circuit Breaker` (abre tras `5` fallos transitorios y se mantiene `30` segundos).
4. `Fallback` controlado para operaciones de lectura (`GET`) cuando se agotan las políticas transitorias.
5. Logging estructurado de eventos de resiliencia (`retry`, `break`, `half-open`, `reset`, `fallback`).
---

## Creación de Base de Datos en MSSQLSERVER

El script de creación está en la raíz del repositorio:

- `script_creacion_dbs_y_tablas.sql`

Pasos sugeridos (SSMS):

1. Abrir SQL Server Mangement Studio.
2. Conectarse a su instancia local de SQL Server.
3. Abrir el archivo `script_creacion_dbs_y_tablas.sql`.
4. Ejecutar el script completo.
5. Ejecutar la siguiente consulta para obtener el nombre del Server:

```sql
SELECT @@SERVERNAME AS 'SERVERNAME'
```

El script crea (si no existen):

1. Base `InventarioProductosBD` con tabla `Productos` e índice `IX_Productos_Nombre`.
2. Base `InventarioTransaccionesBD` con tabla `Transacciones` e índices `IX_Transacciones_ProductoId` y `IX_Transacciones_Fecha`.

Importante:

1. El script es idempotente para tablas e índices (puede ejecutarse varias veces sin duplicar objetos).
2. Revisa/ajusta los connection strings de:
   - `Backend/Microservicios/Sistema.Inventario.Producto/appsettings.json`
   - `Backend/Microservicios/Sistema.Inventario.Transaccion/appsettings.json`

Paso Adicional (cambiar el valor de `Server=` en las cadenas de conexión):

1. Tomar el valor devuelto por la consulta a `@@SERVERNAME` y reemplazarlo en el valor de `Server=` para las cadenas de conexión ubicados en:
    - `Backend/Microservicios/Sistema.Inventario.Producto/appsettings.json`
    - `Backend/Microservicios/Sistema.Inventario.Transaccion/appsettings.json`

> **Nota:** Verificar que el connection string en los archivos `appsettings.json` de cada microservicio apunte a su instancia de SQL Server. Por defecto usa autenticación integrada de Windows sin usuario ni contraseña.

### Alternativa: creación de BD con migraciones EF Core

Si prefieres no ejecutar el script SQL, puedes crear/actualizar las bases desde EF Core con migraciones.

Paso previo:

1. Cambiar el valor de `Server=` en los connection strings de:
    - `Backend/Microservicios/Sistema.Inventario.Producto/appsettings.json`
    - `Backend/Microservicios/Sistema.Inventario.Transaccion/appsettings.json`
2. Usar el valor obtenido con:

```sql
SELECT @@SERVERNAME AS 'SERVERNAME'
```

Comando:

```bash
dotnet tool install --global dotnet-ef
```

Ejemplo de ejecución por microservicio:

### Migraciones - Producto

```bash
cd Backend/Microservicios/Sistema.Inventario.Producto
dotnet ef database update
```

### Migraciones - Transacción

```bash
cd Backend/Microservicios/Sistema.Inventario.Transaccion
dotnet ef database update
```

> **Nota:** Ejecutar `dotnet ef database update` dentro de cada microservicio que tenga `DbContext` y migraciones para crear su base correspondiente (Productos y Transacciones).

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
- Health check: `http://localhost:5261/health`

### 2. Microservicio de Transacciones

```bash
cd Backend/Microservicios/Sistema.Inventario.Transaccion
dotnet run
```

Endpoints útiles:

- API: `http://localhost:5253/api/transacciones`
- Swagger: `http://localhost:5253/swagger`
- Health check: `http://localhost:5253/health`

### 3. API Gateway

```bash
cd Backend/Gateway/Sistema.Inventario.ApiGateway
dotnet run
```

Endpoints útiles:

- Gateway: `http://localhost:7000`
- Swagger Gateway: `http://localhost:7000/swagger`
- Health check: `http://localhost:7000/health`

Rutas principales a través del gateway:

1. `http://localhost:7000/api/productos/*`
2. `http://localhost:7000/api/transacciones/*`

> **Nota:** Los puertos o URLs de salida también pueden consultarse en la carpeta `Properties` de cada proyecto backend, dentro del archivo `launchSettings.json`, en la propiedad `applicationUrl`.

---

## Ejecución del Frontend

**Requisito previo:** Asegurar que el archivo `.env` esté configurado (ver sección de "Variables de entorno Frontend").

```bash
cd Frontend/Sistema.Inventario.App
npm install
npm run dev
```

Aplicación disponible en:

- `http://localhost:5173`

> **Nota:** En backend, los puertos configurados pueden revisarse también en `launchSettings.json`; en frontend, Vite mostrará la URL exacta en la terminal al ejecutar `npm run dev`.

---

## Ejecución de Pruebas

### Backend

```bash
cd Backend/Tests/Sistema.Inventario.Producto.Tests
dotnet test

cd ../Sistema.Inventario.Transaccion.Tests
dotnet test
```

Cobertura backend:

```bash
dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.5.4
```

Ejecutar por cada proyecto de pruebas backend (Producto y Transacción):

### Cobertura backend - Producto

```bash
cd Backend/Tests/Sistema.Inventario.Producto.Tests
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coverage-report" -reporttypes:Html;
```

### Cobertura backend - Transacción

```bash
cd Backend/Tests/Sistema.Inventario.Transaccion.Tests
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coverage-report" -reporttypes:Html;
```

Reportes generados en:

- `Backend/Tests/Sistema.Inventario.Producto.Tests/coverage.cobertura.xml`
- `Backend/Tests/Sistema.Inventario.Producto.Tests/coverage-report/index.html`
- `Backend/Tests/Sistema.Inventario.Transaccion.Tests/coverage.cobertura.xml`
- `Backend/Tests/Sistema.Inventario.Transaccion.Tests/coverage-report/index.html`

Para visualizar la cobertura backend, abrir en navegador:

- `Backend/Tests/Sistema.Inventario.Producto.Tests/coverage-report/index.html`
- `Backend/Tests/Sistema.Inventario.Transaccion.Tests/coverage-report/index.html`

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

> **Nota:** Se agregan capturas de pantalla de la aplicación en funcionamiento a continuación.

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

---

## Casos de Prueba Funcionales (Probados)

| Número | Nombre | Descripción | Precondiciones |Pasos | Resultado esperado | Resultado actual | Estado |
|---:|---|---|---|---|---|---|---|
| 1 | Crear un nuevo producto con imagen local | Probar la creación de un Producto con una imagen local. | 1. Tener levantado el microservicio de Productos, el ApiGateway y el Frontend.<br>2. Tener una imagen local válida lista para cargar. | 1. Ir a Crear Producto.<br>2. Cargar una imagen local válida.<br>3. Completar campos obligatorios y guardar. | El usuario debe observar el producto creado en la tabla de Productos con la imagen correcta. | El usuario observa el producto creado en la tabla de Productos con la imagen correcta. | Aprobado |
| 2 | Crear un nuevo producto con imagen URL | Validar la creación de un Producto con una imagen en URL. | 1. Tener levantado el microservicio de Productos, el ApiGateway y el Frontend.<br>2. Tener una URL de imagen válida disponible. | 1. Ir a Crear Producto.<br>2. Ingresar una URL de imagen válida.<br>3. Completar campos obligatorios y guardar. | El usuario debe observar el producto creado en la tabla de Productos con la imagen correcta. | El usuario observa el producto creado en la tabla de Productos con la imagen correcta. | Aprobado |
| 3 | Consultar un producto | Verificar la visualización de los datos del producto al consultarlo. | 1. Tener levantado el microservicio de Productos, el ApiGateway y el Frontend.<br>2. Tener al menos un producto creado. | 1. Ir al listado de Productos.<br>2. Seleccionar un producto existente y en acciones click en Ver.<br>3. Revisar campos mostrados en pantalla. | El usuario debe poder visualizar la pantalla de Consulta de Producto con los datos del producto consultado. | El usuario visualiza la pantalla de Consulta de Producto con los datos del producto consultado. | Aprobado |
| 4 | Editar un producto cambiar imagen local por URL | Comprobar la actualización de imagen local a imagen URL de un producto. | 1. Tener levantado el microservicio de Productos, el ApiGateway y el Frontend.<br>2. Tener un producto creado con imagen local.<br>3. Tener una URL de imagen válida disponible. | 1. Abrir edición de un producto con imagen local.<br>2. Reemplazar imagen por una URL válida.<br>3. Guardar cambios y volver al listado. | El usuario debe visualizar el producto actualizado en la tabla de Productos con la imagen actualizada. | El usuario visualiza el producto actualizado en la tabla de Productos con la imagen actualizada. | Aprobado |
| 5 | Editar un producto cambiar imagen URL por local | Probar la actualización de imagen URL a imagen local de un producto. | 1. Tener levantado el microservicio de Productos, el ApiGateway y el Frontend.<br>2. Tener un producto creado con imagen URL.<br>3. Tener una imagen local válida lista para cargar. | 1. Abrir edición de un producto con imagen URL.<br>2. Cargar una imagen local válida en reemplazo.<br>3. Guardar cambios y validar en listado. | El usuario debe visualizar el producto actualizado en la tabla de Productos con la imagen actualizada. | El usuario visualiza el producto actualizado en la tabla de Productos con la imagen actualizada. | Aprobado |
| 6 | Eliminar un producto | Validar la eliminación de un producto. | 1. Tener levantado el microservicio de Productos, el ApiGateway y el Frontend.<br>2. Tener al menos un producto creado. | 1. Ir al listado de Productos.<br>2. Seleccionar Eliminar sobre un producto existente.<br>3. Confirmar eliminación y recargar listado. | El usuario no debe observar el producto eliminado en la tabla de Productos. | El usuario no observa el producto eliminado en la tabla de Productos. | Aprobado |
| 7 | Filtrar productos | Verificar el filtrado de productos. | 1. Tener levantado el microservicio de Productos, el ApiGateway y el Frontend.<br>2. Tener varios productos creados con datos distintos. | 1. Ir al listado de Productos.<br>2. Aplicar filtros (nombre/categoría/precio/stock).<br>3. Verificar resultados filtrados. | El usuario debe visualizar los productos que cumplen con los filtros aplicados. | El usuario visualiza los productos que cumplen con los filtros aplicados. | Aprobado |
| 8 | Crear una transacción de venta | Comprobar la realización de una transacción de tipo Venta. | 1. Tener levantados los microservicios de Productos y Transacciones, el ApiGateway y el Frontend.<br>2. Tener un producto creado con stock suficiente. | 1. Ir a Crear Transacción.<br>2. Seleccionar tipo Venta y producto.<br>3. Ingresar cantidad/valor y guardar. | El usuario debe observar la transacción creada tipo Venta en la tabla de Transacciones. | El usuario observa la transacción creada tipo Venta en la tabla de Transacciones. | Aprobado |
| 9 | Crear una transacción de compra | Probar la realización de una transacción de tipo Compra. | 1. Tener levantados los microservicios de Productos y Transacciones, el ApiGateway y el Frontend.<br>2. Tener un producto creado. | 1. Ir a Crear Transacción.<br>2. Seleccionar tipo Compra y producto.<br>3. Ingresar cantidad/valor y guardar. | El usuario debe observar la transacción creada tipo Compra en la tabla de Transacciones. | El usuario observa la transacción creada tipo Compra en la tabla de Transacciones. | Aprobado |
| 10 | Editar una transacción | Validar la edición de los datos de una transacción. | 1. Tener levantado el microservicio de Transacciones, el ApiGateway y el Frontend.<br>2. Tener al menos una transacción creada. | 1. Abrir edición de una transacción existente.<br>2. Modificar los datos permitidos.<br>3. Guardar y validar en el listado. | El usuario debe visualizar la transacción actualizada en la tabla de Transacciones. | El usuario visualiza la transacción actualizada en la tabla de Transacciones. | Aprobado |
| 11 | Eliminar una transacción | Verificar la eliminación de una transacción. | 1. Tener levantado el microservicio de Transacciones, el ApiGateway y el Frontend.<br>2. Tener al menos una transacción creada. | 1. Ir al listado de Transacciones.<br>2. Seleccionar Eliminar en una transacción existente.<br>3. Confirmar y validar que no aparezca. | El usuario no debe observar la transacción eliminada en la tabla de Transacciones. | El usuario no observa la transacción eliminada en la tabla de Transacciones. | Aprobado |
| 12 | Filtrar transacciones | Comprobar el filtrado de transacciones. | 1. Tener levantado el microservicio de Transacciones, el ApiGateway y el Frontend.<br>2. Tener varias transacciones creadas con datos distintos. | 1. Ir al listado de Transacciones.<br>2. Aplicar filtros (tipo, producto, fecha, cantidad).<br>3. Verificar resultados filtrados. | El usuario debe visualizar las transacciones que cumplen con los filtros aplicados. | El usuario visualiza las transacciones que cumplen con los filtros aplicados. | Aprobado |
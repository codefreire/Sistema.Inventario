--Crear base de datos para el microservicio de Productos (si existe no lo crea)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'InventarioProductosBD')
BEGIN
	CREATE DATABASE InventarioProductosBD;
END
GO

--Ir a la base de datos InventarioProductosBD
USE InventarioProductosBD
GO

--Crear tabla Productos (si existe no lo crea)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productos')
BEGIN
	CREATE TABLE Productos(
		Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
		Nombre NVARCHAR(50) NOT NULL,
		Descripcion NVARCHAR(500) NOT NULL,
		Categoria NVARCHAR(50) NOT NULL,
		ImagenUrl NVARCHAR(500) NOT NULL,
		Precio DECIMAL(10,2) NOT NULL CHECK (Precio >= 0),
		Stock INT NOT NULL CHECK (Stock >= 0)
	);
END
GO

--Crear índice en la columna Nombre de la tabla Productos (si existe no lo crea)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Productos_Nombre' AND object_id = OBJECT_ID('dbo.Productos'))
BEGIN
	CREATE INDEX IX_Productos_Nombre
	ON dbo.Productos(Nombre);
END
GO

--Crear base de datos para el microservicio de Transacciones (si existe no lo crea)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'InventarioTransaccionesBD')
BEGIN
	CREATE DATABASE InventarioTransaccionesBD;
END
GO

--Ir a la base de datos InventarioTransaccionesBD
USE InventarioTransaccionesBD
GO

--Crear tabla Transacciones (si existe no lo crea)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Transacciones')
BEGIN
	CREATE TABLE Transacciones(
		Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
		Fecha DATETIME NOT NULL DEFAULT GETDATE(),
		TipoTransaccion NVARCHAR(10) NOT NULL CHECK (TipoTransaccion IN ('Compra', 'Venta')),
		ProductoId UNIQUEIDENTIFIER NOT NULL,
		Cantidad INT NOT NULL CHECK (Cantidad > 0),
		PrecioUnitario DECIMAL(10,2) NOT NULL CHECK (PrecioUnitario >= 0),
		PrecioTotal AS CAST((Cantidad * PrecioUnitario) AS DECIMAL(10,2)) PERSISTED NOT NULL,
		Detalle NVARCHAR(500) NOT NULL
	);
END
GO

--Crear índice en la columna ProductoId de la tabla Transacciones (si existe no lo crea)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Transacciones_ProductoId' AND object_id = OBJECT_ID('dbo.Transacciones'))
BEGIN
	CREATE INDEX IX_Transacciones_ProductoId
	ON dbo.Transacciones(ProductoId);
END
GO

--Crear índice en la columna Fecha de la tabla Transacciones (si existe no lo crea)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Transacciones_Fecha' AND object_id = OBJECT_ID('dbo.Transacciones'))
BEGIN
	CREATE INDEX IX_Transacciones_Fecha
	ON dbo.Transacciones(Fecha);
END
GO
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema.Inventario.Transaccion.Migrations
{
    /// <inheritdoc />
    public partial class EsquemaInicialInventarioTransaccionesBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transacciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TipoTransaccion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrecioTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false, computedColumnSql: "CAST((Cantidad * PrecioUnitario) AS DECIMAL(10,2))", stored: true),
                    Detalle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacciones", x => x.Id);
                    table.CheckConstraint("CK_Transacciones_Cantidad", "[Cantidad] > 0");
                    table.CheckConstraint("CK_Transacciones_PrecioUnitario", "[PrecioUnitario] >= 0");
                    table.CheckConstraint("CK_Transacciones_TipoTransaccion", "[TipoTransaccion] IN (N'Compra', N'Venta')");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transacciones");
        }
    }
}

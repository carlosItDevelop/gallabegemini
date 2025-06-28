using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralLabSolutions.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class VendedorRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataInclusao",
                table: "Vendedor",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaModificacao",
                table: "Vendedor",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioInclusao",
                table: "Vendedor",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioUltimaModificacao",
                table: "Vendedor",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataInclusao",
                table: "Vendedor");

            migrationBuilder.DropColumn(
                name: "DataUltimaModificacao",
                table: "Vendedor");

            migrationBuilder.DropColumn(
                name: "UsuarioInclusao",
                table: "Vendedor");

            migrationBuilder.DropColumn(
                name: "UsuarioUltimaModificacao",
                table: "Vendedor");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralLabSolutions.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class FornecedorHerdaDeEntityAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataInclusao",
                table: "Fornecedor",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaModificacao",
                table: "Fornecedor",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioInclusao",
                table: "Fornecedor",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioUltimaModificacao",
                table: "Fornecedor",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataInclusao",
                table: "Fornecedor");

            migrationBuilder.DropColumn(
                name: "DataUltimaModificacao",
                table: "Fornecedor");

            migrationBuilder.DropColumn(
                name: "UsuarioInclusao",
                table: "Fornecedor");

            migrationBuilder.DropColumn(
                name: "UsuarioUltimaModificacao",
                table: "Fornecedor");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralLabSolutions.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class updAttachementAndComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataInclusao",
                table: "LeadNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaModificacao",
                table: "LeadNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioInclusao",
                table: "LeadNotes",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioUltimaModificacao",
                table: "LeadNotes",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInclusao",
                table: "CrmTaskComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaModificacao",
                table: "CrmTaskComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioInclusao",
                table: "CrmTaskComments",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioUltimaModificacao",
                table: "CrmTaskComments",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInclusao",
                table: "CrmTaskAttachments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaModificacao",
                table: "CrmTaskAttachments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioInclusao",
                table: "CrmTaskAttachments",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioUltimaModificacao",
                table: "CrmTaskAttachments",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataInclusao",
                table: "LeadNotes");

            migrationBuilder.DropColumn(
                name: "DataUltimaModificacao",
                table: "LeadNotes");

            migrationBuilder.DropColumn(
                name: "UsuarioInclusao",
                table: "LeadNotes");

            migrationBuilder.DropColumn(
                name: "UsuarioUltimaModificacao",
                table: "LeadNotes");

            migrationBuilder.DropColumn(
                name: "DataInclusao",
                table: "CrmTaskComments");

            migrationBuilder.DropColumn(
                name: "DataUltimaModificacao",
                table: "CrmTaskComments");

            migrationBuilder.DropColumn(
                name: "UsuarioInclusao",
                table: "CrmTaskComments");

            migrationBuilder.DropColumn(
                name: "UsuarioUltimaModificacao",
                table: "CrmTaskComments");

            migrationBuilder.DropColumn(
                name: "DataInclusao",
                table: "CrmTaskAttachments");

            migrationBuilder.DropColumn(
                name: "DataUltimaModificacao",
                table: "CrmTaskAttachments");

            migrationBuilder.DropColumn(
                name: "UsuarioInclusao",
                table: "CrmTaskAttachments");

            migrationBuilder.DropColumn(
                name: "UsuarioUltimaModificacao",
                table: "CrmTaskAttachments");
        }
    }
}

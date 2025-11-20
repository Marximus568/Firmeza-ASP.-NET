using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdminDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewVersionOfDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Cambiar el tipo de la columna existente
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Users",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            // Si tu columna era NULL antes, usa esto en lugar de nullable: false
            // nullable: true
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Volver atrás si es necesario
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaServicios.Api.Autor.Migrations
{
    public partial class ChangeColumnsName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AutorLigroGuid",
                table: "AutorLibros",
                newName: "AutorLibroGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AutorLibroGuid",
                table: "AutorLibros",
                newName: "AutorLigroGuid");
        }
    }
}

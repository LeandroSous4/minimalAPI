using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimalAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmintrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
        table: "Administradores",
        columns: new[] { "id", "Email", "Senha", "Perfil" },
        values: new object[] { 1, "admin", "admin", "ADM" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

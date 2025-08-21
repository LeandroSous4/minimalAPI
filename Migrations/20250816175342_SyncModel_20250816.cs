using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimalAPI.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_20250816 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "id", "Email", "Perfil", "Senha" },
                values: new object[] { 2, "admin", "ADM", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Administradores",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}

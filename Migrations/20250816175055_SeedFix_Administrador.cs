using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimalAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedFix_Administrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT 1 FROM [Administradores] WHERE [id] = 1)
        BEGIN
            SET IDENTITY_INSERT [Administradores] ON;
            INSERT INTO [Administradores] ([id], [Email], [Senha], [Perfil])
            VALUES (1, 'admin', 'admin', 'ADM');
            SET IDENTITY_INSERT [Administradores] OFF;
        END
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

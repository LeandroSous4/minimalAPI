using Microsoft.VisualStudio.TestTools.UnitTesting;
using minimalAPI.Dominio.Servicos;
using minimalAPI.Dominio.Entidades;
using minimalAPI.Dominio.DTO;
using minimalAPI.Infra.DB;
using System.Linq;
using minimalAPI.DTO;
using Test.Domain.Infra.DB;

namespace minimalAPI.Tests
{
    [TestClass]
    public class AdministradorServicoTests
    {
        private static AdministradorServico BuildService(out DbContexto ctx)
        {
            ctx = DbContextFactory.CreateInMemory();
            return new AdministradorServico(ctx);
        }

        [TestMethod]
        public void Incluir_DevePersistirAdministrador()
        {
            // Arrange
            var serv = BuildService(out var ctx);
            var admin = new Administrador { Email = "admin", Senha = "admin", Perfil = "ADM" };

            // Act
            var salvo = serv.Incluir(admin);

            // Assert
            Assert.AreNotEqual(0, salvo.id, "Id deve ser gerado ao salvar.");
            Assert.AreEqual(1, ctx.Administradores.Count(), "Deveria haver 1 registro no contexto.");
            Assert.AreEqual("admin", ctx.Administradores.Single().Email);
        }

        [TestMethod]
        public void BuscaPorId_DeveRetornarQuandoExiste()
        {
            // Arrange
            var serv = BuildService(out var ctx);
            var a = new Administrador { Email = "x@x", Senha = "123", Perfil = "ADM" };
            ctx.Administradores.Add(a);
            ctx.SaveChanges();

            // Act
            var encontrado = serv.BuscaPorId(a.id);

            // Assert
            Assert.IsNotNull(encontrado);
            Assert.AreEqual(a.id, encontrado!.id);
            Assert.AreEqual("x@x", encontrado.Email);
        }

        [TestMethod]
        public void BuscaPorId_DeveRetornarNullQuandoNaoExiste()
        {
            var serv = BuildService(out var _);
            var encontrado = serv.BuscaPorId(999);
            Assert.IsNull(encontrado);
        }

        [TestMethod]
        public void Login_DeveRetornarQuandoEmailESenhaConferem()
        {
            // Arrange
            var serv = BuildService(out var ctx);
            ctx.Administradores.Add(new Administrador { Email = "a@a", Senha = "123", Perfil = "ADM" });
            ctx.SaveChanges();

            // Act
            var usuario = serv.Login(new LoginDTO { Email = "a@a", Senha = "123" });

            // Assert
            Assert.IsNotNull(usuario);
            Assert.AreEqual("a@a", usuario!.Email);
        }

        [TestMethod]
        public void Login_DeveRetornarNullQuandoSenhaErrada()
        {
            var serv = BuildService(out var ctx);
            ctx.Administradores.Add(new Administrador { Email = "a@a", Senha = "123", Perfil = "ADM" });
            ctx.SaveChanges();

            var usuario = serv.Login(new LoginDTO { Email = "a@a", Senha = "errada" });

            Assert.IsNull(usuario);
        }

        [TestMethod]
        public void Todos_DevePaginar_DezPorPagina()
        {
            // Arrange
            var serv = BuildService(out var ctx);
            for (int i = 1; i <= 25; i++)
                ctx.Administradores.Add(new Administrador { Email = $"u{i}@a", Senha = "x", Perfil = "ADM" });
            ctx.SaveChanges();

            // Act
            var pagina1 = serv.Todos(1);
            var pagina2 = serv.Todos(2);
            var pagina3 = serv.Todos(3);

            // Assert
            Assert.AreEqual(10, pagina1.Count, "Página 1 deveria ter 10 itens.");
            Assert.AreEqual(10, pagina2.Count, "Página 2 deveria ter 10 itens.");
            Assert.AreEqual(5,  pagina3.Count, "Página 3 deveria ter 5 itens (resto).");
        }

        [TestMethod]
        public void Todos_DeveAssumirPagina1QuandoNull()
        {
            // Arrange
            var serv = BuildService(out var ctx);
            for (int i = 1; i <= 12; i++)
                ctx.Administradores.Add(new Administrador { Email = $"u{i}@a", Senha = "x", Perfil = "ADM" });
            ctx.SaveChanges();

            // Act
            var paginaNull = serv.Todos(null);

            // Assert
            Assert.AreEqual(10, paginaNull.Count, "Página null deve se comportar como página 1 (10 itens).");
        }
    }
}

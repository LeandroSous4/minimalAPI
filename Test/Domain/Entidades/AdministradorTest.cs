using minimalAPI.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public sealed class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var adm = new Administrador();

        //Act
        adm.id = 1;
        adm.Email = "admin";
        adm.Senha = "admin";
        adm.Perfil = "Adm";

        // Assert
        Assert.AreEqual(1, adm.id);
        Assert.AreEqual("admin", adm.Email);
        Assert.AreEqual("admin", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalAPI.Dominio.DTO;
using minimalAPI.Dominio.Entidades;
using minimalAPI.DTO;

namespace minimalAPI.Dominio.Interfaces
{
    public interface IAdminitradorServico
    {
        Administrador? Login(LoginDTO login);
        Administrador Incluir(Administrador administrador);
        Administrador? BuscaPorId(int id);
        List<Administrador> Todos(int? pagina);
    }
}
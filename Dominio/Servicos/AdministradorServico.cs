using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using minimalAPI.Dominio.DTO;
using minimalAPI.Dominio.Entidades;
using minimalAPI.Dominio.Interfaces;
using minimalAPI.DTO;
using minimalAPI.Infra.DB;

namespace minimalAPI.Dominio.Servicos
{
    public class AdministradorServico : IAdminitradorServico
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto _db)
        {
            _contexto = _db;
        }

        public Administrador? BuscaPorId(int id)
        {
            return _contexto.Administradores.Where(v => v.id == id).FirstOrDefault();
        }

        public Administrador Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();

            return administrador;
        }

        public Administrador? Login(LoginDTO login)
        {
            return _contexto.Administradores.Where(a => a.Email == login.Email && a.Senha == login.Senha).FirstOrDefault();
        }

        public List<Administrador> Todos(int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();
            
            int itensPorPagina = 10;

            query = query.Skip(((pagina == null ? 1 : (int)pagina) - 1) * itensPorPagina).Take(itensPorPagina);

            return query.ToList();
        }
    }
}
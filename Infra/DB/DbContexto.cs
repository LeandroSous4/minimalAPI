using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimalAPI.Dominio.Entidades;

namespace minimalAPI.Infra.DB
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configuracao;
        public DbContexto(IConfiguration configuration){
            _configuracao = configuration;
        }
        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    id = 1,
                    Email = "admin",
                    Senha = "admin",
                    Perfil = "ADM"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connection = _configuracao.GetConnectionString("sqlServer");
                if (!string.IsNullOrEmpty(connection))
                {
                    optionsBuilder.UseSqlServer(connection);
                }
            }
        }
    }
}
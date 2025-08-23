using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalAPI.Dominio.Enum;

namespace minimalAPI.Dominio.ModelViews
{
    public record AdministradorModel
    {
        public int id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Perfil { get; set; } = default!;
    }
}
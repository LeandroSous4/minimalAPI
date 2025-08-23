using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimalAPI.Dominio.ModelViews
{
    public struct Home
    {
        public string Documentacao { get => "/swagger"; }
        public string Mensagem { get => "Bem vindo a API de veiculos"; }
    }
}
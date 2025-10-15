using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atleta.Core.Models
{
    public class EnderecoDto
    {
        public string CEP { get; set; } = "";
        public string Rua { get; set; } = "";
        public int Numero { get; set; }
        public string Bairro { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Pais { get; set; } = "";
    }
}
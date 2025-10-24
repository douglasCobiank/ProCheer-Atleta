using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atleta.Core.Models
{
    public class DocumentoDto
    {
        public int Id { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string Passaporte { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atleta.Core.Models
{
    public class AtletasDto
    {
        public int Id { get; set; }

        public int? UsuarioId { get; set; }
        public UsuarioDto Usuario { get; set; }
        public List<GinasioDto> Ginasio { get; set; }

        public int? DocumentoId { get; set; }
        public DocumentoDto Documento { get; set; }

        public string ComprovanteMatricula { get; set; }
        public int Genero { get; set; }
        public int Idade { get; set; }
        public string? ImagemAtleta { get; set; }
    }
}
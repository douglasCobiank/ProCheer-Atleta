namespace Atleta.Core.Models
{
    public class GinasioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public EnderecoDto Endereco { get; set; } = new EnderecoDto();
        public string Responsavel { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Email { get; set; } = "";
        public int Tipo { get; set; }
        public string? ImagemLogo { get; set; }  
    }
}
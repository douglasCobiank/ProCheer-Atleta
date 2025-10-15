namespace Atleta.Api.Models
{
    public class Ginasio
    {
        public string Nome { get; set; } = "";
        public Endereco Endereco { get; set; } = new Endereco();
        public string Responsavel { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Email { get; set; } = "";
        public int Tipo { get; set; }
    }
}
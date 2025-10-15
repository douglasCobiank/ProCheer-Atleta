namespace Atleta.Api.Models
{
    public class Atletas
    {
        public Usuario Usuario { get; set; }
        public Documento Documento { get; set; }
        public List<Ginasio> Ginasio { get; set; }
        public string ComprovanteMatricula { get; set; }
        public int Genero { get; set; }
        public int Idade { get; set; }
    }
}
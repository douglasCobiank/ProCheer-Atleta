namespace Atleta.Core.Models
{
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = "Atleta"; 
    }
}
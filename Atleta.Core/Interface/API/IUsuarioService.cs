using Atleta.Core.Models;
using Refit;

namespace Atleta.Core.Interface.API
{
    public interface IUsuarioService
    {
        [Post("/api/Usuario/cadastrar")]
        Task CadastrarUsuarioAsync(UsuarioDto usuarioDto);

        [Post("/api/Usuario/buscar-usuario-login")]
        Task<List<UsuarioDto>> GetUsuarioPorLoginAsync(string login, string senha);

        [Post("/api/Usuario/buscar-usuario-id/{id}")]
        Task<List<UsuarioDto>> GetUsuarioPorIdAsync(int id);

        [Post("/api/Usuario/buscar-usuario-nome/{nome}")]
        Task<List<UsuarioDto>> GetUsuarioPorNomeAsync(string nome);
    }
}
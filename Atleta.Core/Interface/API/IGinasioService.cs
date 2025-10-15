using Atleta.Core.Models;
using Refit;

namespace Atleta.Core.Interface.API
{
    public interface IGinasioService
    {
        [Post("/api/Ginasio/cadastrar")]
        Task CadastrarGinasioAsync(GinasioDto ginasioDto);

        [Post("/api/Ginasio/buscar-ginasio-por-nome/{nome}")]
        Task<List<GinasioDto>> GetGinasioPorNomeAsync(string nome);

        [Post("/api/Ginasio/buscar-ginasio-por-id/{id}")]
        Task<List<GinasioDto>> GetGinasioAsync(int id);
    }
}
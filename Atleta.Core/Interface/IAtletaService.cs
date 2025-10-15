using Atleta.Core.Models;

namespace Atleta.Core.Interface
{
    public interface IAtletaService
    {
        Task AddAtletaAsync(AtletasDto atletaDto);
        Task DeletaAtletaAsync(int id);
        Task EditarAtletaAsync(AtletasDto atletaDto, int id);
        Task<List<AtletasDto>> GetAtletaAsync();
        Task<AtletasDto> GetAtletaPorNomeAsync(string nome);
        Task AdicionaLogoGinasioAsync(int id, string imagem);
    }
}
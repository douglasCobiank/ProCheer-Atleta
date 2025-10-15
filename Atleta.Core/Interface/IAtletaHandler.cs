using Atleta.Core.Models;

namespace Atleta.Core.Interface
{
    public interface IAtletaHandler
    {
        Task<List<AtletasDto>> BuscaAtletaAsync();
        Task<AtletasDto> BuscaAtletaPorNomeAsync(string nome);
        Task CadastrarAtletaHandler(AtletasDto atletaDto);
        Task DeletaAtletaAsync(int id);
        Task EditarAtletaAsync(AtletasDto atletaDto, int id);
        Task AdicionaImagemAtletaAsync(int id, string imagemBytes);
    }
}
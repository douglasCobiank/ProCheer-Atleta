using Atleta.Core.Interface;
using Atleta.Core.Models;
using Microsoft.Extensions.Logging;

namespace Atleta.Core
{
    public class AtletaHandler(IAtletaService atletaService, ILogger<AtletaHandler> logger) : IAtletaHandler
    {
        private readonly IAtletaService _atletaService = atletaService;
        private readonly ILogger<AtletaHandler> _logger = logger;

        public async Task<List<AtletasDto>> BuscaAtletaAsync()
        {
            try
            {
                _logger.LogInformation("Buscando todos os atletas...");
                var atletas = await _atletaService.GetAtletaAsync();
                _logger.LogInformation("{Count} atletas encontrados.", atletas.Count);
                return atletas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar lista de atletas.");
                throw;
            }
        }

        public async Task<AtletasDto> BuscaAtletaPorNomeAsync(string nome)
        {
            try
            {
                _logger.LogInformation("Buscando atleta pelo nome: {Nome}", nome);               
                var atleta = await _atletaService.GetAtletaPorNomeAsync(nome);

                if (atleta is null)
                    _logger.LogWarning("Atleta com o nome {Nome} não encontrado.", nome);
                else
                    _logger.LogInformation("Atleta {Nome} encontrado com sucesso.", nome);

                return atleta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar atleta pelo nome: {Nome}", nome);
                throw;
            }
        }

        public async Task CadastrarAtletaHandler(AtletasDto atletaDto)
        {
            try
            {
                _logger.LogInformation("Iniciando cadastro do atleta: {Nome}", atletaDto.Usuario.Nome);
                await _atletaService.AddAtletaAsync(atletaDto);
                _logger.LogInformation("Atleta {Nome} cadastrado com sucesso.", atletaDto.Usuario.Nome);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar atleta: {Nome}", atletaDto.Usuario.Nome);
                throw;
            }
        }

        public async Task DeletaAtletaAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deletando atleta com ID: {Id}", id);
                await _atletaService.DeletaAtletaAsync(id);
                _logger.LogInformation("Atleta com ID {Id} deletado com sucesso.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar atleta com ID {Id}", id);
                throw;
            }
        }

        public async Task EditarAtletaAsync(AtletasDto atletaDto, int id)
        {
            try
            {
                _logger.LogInformation("Editando atleta com ID: {Id}", id);
                await _atletaService.EditarAtletaAsync(atletaDto, id);
                _logger.LogInformation("Atleta {Nome} atualizado com sucesso.", atletaDto.Usuario.Nome);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar atleta com ID {Id}", id);
                throw;
            }
        }

        public async Task AdicionaImagemAtletaAsync(int id, string imagemBytes)
        {
            try
            {
                _logger.LogInformation("Adicionando imagem para atleta ID: {Id}", id);
                await _atletaService.AdicionaLogoGinasioAsync(id, imagemBytes);
                _logger.LogInformation("Imagem adicionada ao atleta com ID {Id} com sucesso.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar imagem para atleta ID {Id}", id);
                throw;
            }
        }
    }
}

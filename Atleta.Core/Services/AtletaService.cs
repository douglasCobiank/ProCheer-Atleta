using Atleta.Core.Interface;
using Atleta.Core.Interface.API;
using Atleta.Core.Models;
using Atleta.Infrastructure.Cache;
using Atleta.Infrastructure.Data.Models;
using Atleta.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Atleta.Core.Services
{
    public class AtletaService(
        IAtletaRepository atletaRepository,
        IMapper mapper,
        IUsuarioService usuarioService,
        IGinasioService ginasioService,
        ILogger<AtletaService> logger,
        ICacheService cacheService,
        IMensageriaService mensageriaService) : IAtletaService
    {
        private readonly IAtletaRepository _atletaRepository = atletaRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly IGinasioService _ginasioService = ginasioService;
        private readonly ILogger<AtletaService> _logger = logger;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IMensageriaService _mensageria = mensageriaService;

        public async Task AddAtletaAsync(AtletasDto atletaDto)
        {
            _logger.LogInformation("Iniciando cadastro de atleta: {Nome}", atletaDto.Usuario.Nome);

            atletaDto.Ginasio = await ObterOuCriarGinasiosAsync(atletaDto.Ginasio);
            
            atletaDto.Usuario = await GarantirUsuarioExistente(atletaDto);
            atletaDto.UsuarioId = atletaDto.Usuario.UsuarioId;

            var atletaData = MapearAtleta(atletaDto);

            atletaDto.Ginasio.ForEach(a =>
            {
                atletaData.AtletaGinasios.Add(new AtletaGinasio
                {
                    Atleta = atletaData,
                    Ginasio = _mapper.Map<GinasioData>(a)
                });
            });
            
            await _atletaRepository.AddAsync(atletaData);
            await AtualizarCache(atletaDto.Usuario.Nome, atletaDto);

            _logger.LogInformation("Atleta {Nome} cadastrado com sucesso", atletaDto.Usuario.Nome);
        }

        public async Task DeletaAtletaAsync(int id)
        {
            _logger.LogInformation("Tentando deletar atleta com ID: {Id}", id);

            var atleta = await _atletaRepository.GetByIdAsync(id);
            if (atleta is null)
            {
                _logger.LogWarning("Atleta com ID {Id} não encontrado para exclusão", id);
                return;
            }

            var usuario = (await _usuarioService.GetUsuarioPorIdAsync(atleta.Usuario.UsuarioId)).FirstOrDefault();
            await RemoverCache(usuario?.Nome);

            await _atletaRepository.DeleteAsync(atleta);
            _logger.LogInformation("Atleta com ID {Id} deletado com sucesso", id);
        }

        public async Task EditarAtletaAsync(AtletasDto atletaDto, int id)
        {
            _logger.LogInformation("Editando atleta com ID: {Id}", id);

            var atleta = await _atletaRepository.GetByIdAsync(id);
            if (atleta is null)
            {
                _logger.LogWarning("Atleta com ID {Id} não encontrado para edição", id);
                return;
            }

            _mapper.Map(atletaDto, atleta);
            atleta.Documento = _mapper.Map<DocumentoData>(atletaDto.Documento);

            await _atletaRepository.EditAsync(atleta);
            await RemoverCache(atletaDto.Usuario.Nome);

            _logger.LogInformation("Atleta {Nome} atualizado com sucesso", atletaDto.Usuario.Nome);
        }

        public async Task<List<AtletasDto>> GetAtletaAsync()
        {
            const string cacheKey = "atletas_todos";

            var cached = await _cacheService.GetAsync<List<AtletasDto>>(cacheKey);
            if (cached is not null) return cached;

            var atletas = await _atletaRepository.GetAllWithIncludeAsync();



            var atletasDto = _mapper.Map<List<AtletasDto>>(atletas);
            await _cacheService.SetAsync(cacheKey, atletasDto, TimeSpan.FromMinutes(10));

            return atletasDto;
        }

        public async Task<AtletasDto> GetAtletaPorNomeAsync(string nome)
        {
            var cacheKey = $"atleta_{nome.ToLower()}";

            var cached = await _cacheService.GetAsync<AtletasDto>(cacheKey);
            if (cached is not null) return cached;

            var usuario = (await _usuarioService.GetUsuarioPorNomeAsync(nome)).FirstOrDefault();
            if (usuario is null)
            {
                _logger.LogWarning("Nenhum atleta encontrado com o nome: {Nome}", nome);
                return null;
            }

            var atleta = await _atletaRepository.GetByIdAsync(usuario.UsuarioId);

            var dto = _mapper.Map<AtletasDto>(atleta);
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

            return dto;
        }

        public async Task AdicionaLogoGinasioAsync(int id, string imagem)
        {
            var atleta = await _atletaRepository.GetByIdAsync(id);
            if (atleta is null)
            {
                _logger.LogWarning("Atleta com ID {Id} não encontrado para adicionar imagem", id);
                return;
            }

            atleta.ImagemAtleta = imagem;
            await _atletaRepository.EditAsync(atleta);

            _logger.LogInformation("Imagem adicionada ao atleta com ID {Id}", id);
        }

        // ==================== MÉTODOS AUXILIARES ====================

        private AtletaData MapearAtleta(AtletasDto dto)
        {
            var atleta = _mapper.Map<AtletaData>(dto);
            atleta.ImagemAtleta = string.Empty;
            return atleta;
        }

        private async Task<UsuarioDto> GarantirUsuarioExistente(AtletasDto dto)
        {
            var usuario = (await _usuarioService.GetUsuarioPorLoginAsync(dto.Usuario.Login, dto.Usuario.SenhaHash))
                          .FirstOrDefault();

            if (usuario is not null) return usuario;

            _logger.LogInformation("Usuário não encontrado. Cadastrando novo usuário: {Login}", dto.Usuario.Login);
            await CadastrarUsuarioAsync(dto);

            return (await _usuarioService.GetUsuarioPorLoginAsync(dto.Usuario.Login, dto.Usuario.SenhaHash))
                   .FirstOrDefault();
        }

        private async Task AtualizarCache(string nome, AtletasDto atletaDto)
        {
            await RemoverCache(nome);
            _mensageria.PublicarMensagem("atualizar-cache", atletaDto);
        }

        private async Task RemoverCache(string nome)
        {
            await _cacheService.RemoveAsync("atletas_todos");
            if (!string.IsNullOrWhiteSpace(nome))
                await _cacheService.RemoveAsync($"atleta_{nome.ToLower()}");
        }

        private async Task<IEnumerable<GinasioData>> CarregarGinasios(IEnumerable<int> ginasioIds)
        {
            var ginasios = new List<GinasioData>();
            foreach (var id in ginasioIds)
            {
                var response = await _ginasioService.GetGinasioAsync(id);
                if (response is not null)
                    ginasios.AddRange(_mapper.Map<List<GinasioData>>(response));
            }
            return ginasios;
        }

        private async Task<List<GinasioDto>> ObterOuCriarGinasiosAsync(List<GinasioDto> ginasiosDto)
        {
            if (ginasiosDto is null || ginasiosDto.Count == 0)
                return new List<GinasioDto>();

            var tasks = ginasiosDto.Select(async ginasio =>
            {
                var existente = (await _ginasioService.GetGinasioPorNomeAsync(ginasio.Nome)).FirstOrDefault();

                if (existente == null)
                {
                    await CadastrarGinasioAsync(ginasio);
                    existente = (await _ginasioService.GetGinasioPorNomeAsync(ginasio.Nome)).FirstOrDefault();
                }

                return existente!;
            });

            return (await Task.WhenAll(tasks)).Where(x => x != null).ToList();
        }


        private async Task CadastrarUsuarioAsync(AtletasDto dto)
        {
            var usuario = new UsuarioDto
            {
                Login = dto.Usuario.Login,
                SenhaHash = dto.Usuario.SenhaHash,
                Nome = dto.Usuario.Nome,
                Email = dto.Usuario.Email,
                Telefone = dto.Usuario.Telefone,
                TipoUsuario = "Atleta"
            };

            await _usuarioService.CadastrarUsuarioAsync(usuario);
            _logger.LogInformation("Usuário {Login} cadastrado com sucesso", usuario.Login);
        }

        private async Task CadastrarGinasioAsync(GinasioDto dto)
        {
            await _ginasioService.CadastrarGinasioAsync(dto);
            _logger.LogInformation("Ginásio {Nome} cadastrado com sucesso", dto.Nome);
        }
    }
}
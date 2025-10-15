using Atleta.Api.Models;
using Atleta.Core.Interface;
using Atleta.Core.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Atleta.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtletaController(IAtletaHandler atletaHandler, IMapper mapper) : ControllerBase
    {
        private readonly IAtletaHandler _atletaHandler = atletaHandler;
        private readonly IMapper _mapper = mapper;

        [HttpPost("criar-atleta")]
        public async Task<IActionResult> CriarAtletaAsync([FromBody] Atletas atleta)
        {
            if (atleta is null)
                return BadRequest("Dados do atleta inválidos.");

            var atletaDto = _mapper.Map<AtletasDto>(atleta);
            await _atletaHandler.CadastrarAtletaHandler(atletaDto);

            return Created(string.Empty, new { Mensagem = "Atleta criado com sucesso." });
        }

        [HttpPut("editar-atleta/{id:int}")]
        public async Task<IActionResult> EditarAtletaAsync(int id, [FromBody] Atletas atleta)
        {
            if (atleta is null)
                return BadRequest("Dados do atleta inválidos.");

            var atletaDto = _mapper.Map<AtletasDto>(atleta);
            await _atletaHandler.EditarAtletaAsync(atletaDto, id);

            return Ok(new { Mensagem = "Atleta atualizado com sucesso." });
        }

        [HttpDelete("deletar-atleta/{id:int}")]
        public async Task<IActionResult> DeletarAtletaAsync(int id)
        {
            await _atletaHandler.DeletaAtletaAsync(id);
            return Ok(new { Mensagem = "Atleta deletado com sucesso." });
        }

        [HttpGet("buscar-atleta")]
        public async Task<IActionResult> BuscarAtletasAsync()
        {
            var atletas = await _atletaHandler.BuscaAtletaAsync();
            if (atletas == null || atletas.Count == 0)
                return NotFound("Nenhum atleta encontrado.");

            return Ok(atletas);
        }

        [HttpGet("buscar-atleta-por-nome/{nome}")]
        public async Task<IActionResult> BuscarAtletaPorNomeAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("Nome inválido.");

            var atleta = await _atletaHandler.BuscaAtletaPorNomeAsync(nome);
            if (atleta == null)
                return NotFound("Atleta não encontrado.");

            return Ok(atleta);
        }

        [HttpPost("upload-imagem")]
        public async Task<IActionResult> UploadImagemAsync([FromForm] UploadImagem arquivo)
        {
            if (arquivo?.Arquivo == null || arquivo.Arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            using var ms = new MemoryStream();
            await arquivo.Arquivo.CopyToAsync(ms);
            string base64String = Convert.ToBase64String(ms.ToArray());

            await _atletaHandler.AdicionaImagemAtletaAsync(arquivo.Id, base64String);

            return Ok(new
            {
                Mensagem = "Upload realizado com sucesso!",
                Tamanho = arquivo.Arquivo.Length
            });
        }
    }
}

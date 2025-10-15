using Atleta.Api.Models;
using Atleta.Core.Models;
using AutoMapper;

namespace Atleta.Api.Mappers
{
    public class AtletaMapper: Profile
    {
        public AtletaMapper()
        {
            CreateMap<AtletasDto, Atletas>().ReverseMap();
            CreateMap<DocumentoDto, Documento>().ReverseMap();
            CreateMap<EnderecoDto, Endereco>().ReverseMap();
            CreateMap<GinasioDto, Ginasio>().ReverseMap();
            CreateMap<UsuarioDto, Usuario>().ReverseMap();
        }
    }
}
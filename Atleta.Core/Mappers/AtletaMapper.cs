using Atleta.Core.Models;
using Atleta.Infrastructure.Data.Models;
using AutoMapper;

namespace Atleta.Core.Mappers
{
    public class AtletaMapper : Profile
    {
        public AtletaMapper()
        {
            CreateMap<AtletasDto, AtletaData>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())          // Id será gerado pelo banco
            //.ForMember(dest => dest.Ginasios, opt => opt.MapFrom(src => src.Ginasio.Select(g => new GinasioData { Id = g.Id }))) 
            .ForMember(dest => dest.Documento, opt => opt.MapFrom(src => src.Documento)).ReverseMap();

            // Mapeamento de Documento
            CreateMap<DocumentoDto, DocumentoData>().ReverseMap();

            // Se precisar mapear GinasioDto para GinasioData
            CreateMap<GinasioDto, GinasioData>().ReverseMap();

            CreateMap<GinasioData, GinasioDto>().ReverseMap();

            CreateMap<EnderecoData, EnderecoDto>().ReverseMap();

            CreateMap<UsuarioData, UsuarioDto>().ReverseMap();
        }
    }
}
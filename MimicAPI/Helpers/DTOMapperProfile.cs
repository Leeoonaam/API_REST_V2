using AutoMapper;
using MimicAPI.Models;
using MimicAPI.Models.DTO;

namespace MimicAPI.Helpers
{
    public class DTOMapperProfile : Profile //herdar em profile para ser compativel
    {
        // objeto que vai possibilitar o mapeamento
        public DTOMapperProfile() 
        {
            /* Caso tivesse uma propriedade com o nome diferente por exemplo: 
                
                PalavraDTO.Nome = Palavra.Name

            Basta adicionar a configuração por membro, basta fazer dessa forma:
            CreateMap<palavra, PalavraDTO>().ForMember(DTO => DTO.Nome, plavra => plavra.MapFrom(src => src.Name));

             */
            // config para possibilitar a conversao de paravra para palavraDTO
            CreateMap<palavra, PalavraDTO>();

            //mapeamento da lista palavra para lista palavraDTO
            CreateMap<PaginationList<palavra>, PaginationList<PalavraDTO>>();
        }
    }
}

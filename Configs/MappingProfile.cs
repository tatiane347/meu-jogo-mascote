// Configs/MappingProfile.cs
using AutoMapper; // IMPORTANTE: Adicione esta linha (agora que AutoMapper está instalado!)
using MeuJogoMascote.Models; // IMPORTANTE: Adicione esta linha para ver suas classes Mascote e PokemonApiDto

namespace MeuJogoMascote.Configs
{
    // Esta classe ensina o AutoMapper como converter um objeto em outro.
    public class MappingProfile : Profile // Sua classe de perfil DEVE herdar de Profile
    {
        public MappingProfile()
        {
            // Esta linha principal diz ao AutoMapper:
            // "Crie um mapeamento de PokemonApiDto (origem) para Mascote (destino)."
            CreateMap<PokemonApiDto, Mascote>()
                // Agora, dizemos ao AutoMapper o que fazer com as propriedades do Mascote:
                // Para 'Name': Ele tem o mesmo nome em PokemonApiDto e Mascote, então o AutoMapper
                // já saberia mapear automaticamente. Esta linha é mais para clareza ou se os nomes fossem diferentes.
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                // Para as propriedades que SÓ existem no Mascote (Alimentacao, Humor, Sono, etc.):
                // Dizemos ao AutoMapper para IGNORAR a tentativa de mapeamento da origem.
                // Isso significa que estas propriedades NÃO serão preenchidas pelo AutoMapper.
                // Você as preencherá MANUALMENTE com os valores randômicos depois no Controller.
                .ForMember(dest => dest.Alimentacao, opt => opt.Ignore())
                .ForMember(dest => dest.Humor, opt => opt.Ignore())
                .ForMember(dest => dest.Sono, opt => opt.Ignore())
                .ForMember(dest => dest.EstaDormindo, opt => opt.Ignore())
                .ForMember(dest => dest.UltimaInteracao, opt => opt.Ignore());
        }
    }
}
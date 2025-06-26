// Models/PokemonModels.cs

using System; // Necessário para DateTime
using System.Collections.Generic; // Necessário para List<T>
using System.Text.Json.Serialization; // Necessário para JsonPropertyName

namespace MeuJogoMascote.Models // Namespace corrigido
{
    // Classes para mapear a lista geral de Pokémons (Dia 1)
    public class PokemonListItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class PokemonListResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }

        [JsonPropertyName("previous")]
        public object Previous { get; set; }

        [JsonPropertyName("results")]
        public List<PokemonListItem> Results { get; set; }
    }

    // Classes para mapear os detalhes de um Pokémon específico e as NECESSIDADES (Dia 2 e Dia 5)
    public class Mascote
    {
        [JsonPropertyName("abilities")]
        public List<AbilitiesClass> Abilities { get; set; }

        [JsonPropertyName("base_experience")]
        public int BaseExperience { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("weight")]
        public int Weight { get; set; }

        // NOVAS PROPRIEDADES PARA O DIA 5 (necessidades do mascote):
        public int Alimentacao { get; set; } // 0 = muita fome, 10 = bem alimentado
        public int Humor { get; set; }       // 0 = muito triste, 10 = muito alegre
        public int Sono { get; set; }        // 0 = muito sono, 10 = bem descansado

        // Propriedades adicionais que podem ser úteis para interações e tempo
        public bool EstaDormindo { get; set; } // Para controlar o estado de sono
        public DateTime UltimaInteracao { get; set; } // Para simular o passar do tempo
    }

    public class AbilitiesClass
    {
        [JsonPropertyName("ability")]
        public AbilityDetail Ability { get; set; }

        [JsonPropertyName("is_hidden")]
        public bool IsHidden { get; set; }

        [JsonPropertyName("slot")]
        public int Slot { get; set; }
    }

    public class AbilityDetail
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
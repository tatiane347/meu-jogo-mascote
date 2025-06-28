// Models/PokemonApiDto.cs
using System.Text.Json.Serialization; // Necessário para a anotação [JsonPropertyName]
using System.Collections.Generic; // Necessário para List<T>

namespace MeuJogoMascote.Models
{
    // Esta classe representa APENAS os dados brutos de um Pokémon que vêm da PokeAPI.
    // É a "caixa de entrada" que o JsonSerializer vai usar para *ler* o JSON da API.
    public class PokemonApiDto
    {
        [JsonPropertyName("id")] // Isso diz ao C# para pegar o campo "id" do JSON
        public int Id { get; set; }

        [JsonPropertyName("name")] // Pega o campo "name" do JSON
        public string Name { get; set; }

        [JsonPropertyName("height")] // Pega o campo "height" do JSON
        public int Height { get; set; }

        [JsonPropertyName("weight")] // Pega o campo "weight" do JSON
        public int Weight { get; set; }

        [JsonPropertyName("sprites")] // Pega o campo "sprites" do JSON
        public Sprites Sprites { get; set; } // Usa a classe Sprites definida abaixo
    }

    // Esta classe representa a parte "sprites" do JSON de um Pokémon.
    public class Sprites
    {
        [JsonPropertyName("front_default")] // Pega o campo "front_default" do JSON
        public string FrontDefault { get; set; }
    }

    // Esta classe é usada para a lista inicial de Pokémons (se você a usa)
    public class PokemonListResponse
    {
        [JsonPropertyName("results")]
        public List<PokemonResult> Results { get; set; }
    }

    // Esta classe representa cada Pokémon na lista de resultados.
    public class PokemonResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
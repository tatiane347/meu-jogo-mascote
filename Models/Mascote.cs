// Models/Mascote.cs
using System; // Necessário para DateTime

namespace MeuJogoMascote.Models
{
    // Esta é a classe que representa o SEU mascote Tamagotchi no jogo.
    public class Mascote
    {
        // Propriedade que virá da API (o nome do Pokémon que você escolheu)
        public string Name { get; set; }

        // Atributos ESPECÍFICOS do SEU JOGO Tamagotchi (VOCÊ os controla, eles não vêm da API):
        public int Alimentacao { get; set; } // Nível de fome (0 = muita fome, 10 = bem alimentado)
        public int Humor { get; set; }       // Nível de alegria (0 = muito triste, 10 = muito alegre)
        public int Sono { get; set; }        // Nível de cansaço (0 = muito sono, 10 = bem descansado)
        public bool EstaDormindo { get; set; } // Para saber se ele está dormindo
        public DateTime UltimaInteracao { get; set; } // Para controlar o tempo que passou desde a última atualização

        // Construtor vazio. É uma boa prática ter um.
        public Mascote() { }
    }
}
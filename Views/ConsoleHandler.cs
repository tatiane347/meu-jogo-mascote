// Views/ConsoleHandler.cs
using System;
using System.Collections.Generic;
using System.Linq; // Necessário para o .Any() se for usar em outras partes

// Apenas UMA linha para importar o namespace Models é suficiente:
using MeuJogoMascote.Models; // Para poder usar as classes Mascote, PokemonApiDto, PokemonResult etc.

namespace MeuJogoMascote.Views
{
    public class ConsoleHandler
    {
        public void ExibirBoasVindas()
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("    Bem-vindo ao Jogo de Mascotes Virtuais!");
            Console.WriteLine("------------------------------------------");
            Console.Write("Qual é o seu nome, treinador(a)? ");
            string nome = Console.ReadLine();
            Console.WriteLine($"Olá, {nome}! Prepare-se para uma aventura!");
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }

        public void ExibirMenuPrincipalOpcoes()
        {
            Console.WriteLine("\n--- MENU PRINCIPAL ---");
            Console.WriteLine("1. Adoção de Mascotes");
            Console.WriteLine("2. Interagir com Mascote");
            Console.WriteLine("3. Sair do Jogo");
            Console.Write("Escolha uma opção: ");
        }

        public void ExibirStatusMascote(Mascote mascote)
        {
            Console.WriteLine($"\n--- Status de {mascote.Name.ToUpper()} ---");
            Console.WriteLine($"  Alimentação: {mascote.Alimentacao}/10");
            Console.WriteLine($"  Humor:       {mascote.Humor}/10");
            Console.WriteLine($"  Sono:        {mascote.Sono}/10");
            Console.WriteLine($"  Status:      {(mascote.EstaDormindo ? "Dormindo 😴" : "Acordado 😊")}");
            Console.WriteLine("----------------------------------");
        }

        public void ExibirMensagem(string mensagem)
        {
            Console.WriteLine(mensagem);
        }

        public void LimparTela()
        {
            Console.Clear();
        }

        public void ExibirListaPokemons(List<PokemonResult> pokemons)
        {
            Console.WriteLine("\nEspécies Disponíveis para Adoção (primeiros 20):");
            foreach (var pokemon in pokemons)
            {
                Console.WriteLine($"- {pokemon.Name}");
            }
        }

        public void ExibirDetalhesMascote(Mascote mascote)
        {
            Console.WriteLine("\n--- DETALHES DO MASCOTE ---");
            Console.WriteLine($"Nome: {mascote.Name}");
            // As propriedades Height, Weight, BaseExperience e Abilities foram removidas
            // porque não existem mais na classe Mascote, apenas no PokemonApiDto.
            // Apenas o nome do mascote e suas necessidades (status) são relevantes para o Tamagotchi.

            ExibirStatusMascote(mascote); // Reutiliza o método para mostrar status

            // O bloco de Habilidades foi removido.
        }
    }
}
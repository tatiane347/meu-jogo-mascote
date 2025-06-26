// Views/ConsoleHandler.cs
using System;
using System.Collections.Generic;
using System.Linq;

using MeuJogoMascote.Models; // Para poder usar as classes Mascote, PokemonListItem etc.

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

        // Mantenha esta aqui para o menu principal
        public void ExibirMenuPrincipalOpcoes()
        {
            Console.WriteLine("\n--- MENU PRINCIPAL ---");
            Console.WriteLine("1. Adoção de Mascotes");
            Console.WriteLine("2. Interagir com Mascote"); // NOVIDADE DO DIA 5: Nova opção
            Console.WriteLine("3. Sair do Jogo");
            Console.Write("Escolha uma opção: ");
        }

        // NOVO MÉTODO PARA EXIBIR O STATUS DO MASCOTE (DIA 5)
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

        public void ExibirListaPokemons(List<PokemonListItem> pokemons)
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
            Console.WriteLine($"Altura: {mascote.Height} decimetros");
            Console.WriteLine($"Peso: {mascote.Weight} hectogramas");
            Console.WriteLine($"Experiência Base: {mascote.BaseExperience}");

            // NOVIDADE DO DIA 5: Exibir as necessidades iniciais na tela de detalhes
            ExibirStatusMascote(mascote); // Reutiliza o método para mostrar status

            Console.WriteLine("\n--- Habilidades ---");
            if (mascote.Abilities != null && mascote.Abilities.Any())
            {
                foreach (var habilidade in mascote.Abilities)
                {
                    Console.WriteLine($"- {habilidade.Ability.Name} (Escondida: {habilidade.IsHidden})");
                }
            }
            else
            {
                Console.WriteLine("Nenhuma habilidade encontrada para este mascote.");
            }
        }
    }
}
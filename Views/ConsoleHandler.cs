// Views/ConsoleHandler.cs
using System;
using System.Collections.Generic; // Para List<T>
using System.Linq; // Necessário para o método .Any()

// AQUI: Este using deve apontar para o seu Models e estar com os outros usings
using MeuJogoMascote.Models; // Corrigido para 'MeuJogoMascote.Models'

namespace MeuJogoMascote.Views // AQUI: Namespace DEVE ser 'MeuJogoMascote.Views'
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
            Console.WriteLine("2. Ver Mascotes Adotados (em breve!)");
            Console.WriteLine("3. Sair do Jogo");
            Console.Write("Escolha uma opção: ");
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
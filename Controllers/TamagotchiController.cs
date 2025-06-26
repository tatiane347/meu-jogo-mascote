// Controllers/TamagotchiController.cs
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;

using MeuJogoMascote.Models; // AQUI DEVE SER 'MeuJogoMascote'
using MeuJogoMascote.Views;  // AQUI DEVE SER 'MeuJogoMascote'

namespace MeuJogoMascote.Controllers // AQUI DEVE SER 'MeuJogoMascote'
{
    public class TamagotchiController
    {
        private ConsoleHandler _view;
        private static readonly HttpClient _httpClient = new HttpClient();

        public TamagotchiController(ConsoleHandler view)
        {
            _view = view;
        }

        public async Task Jogar()
        {
            _view.ExibirBoasVindas();

            int opcao = 0;
            while (opcao != 3)
            {
                _view.ExibirMenuPrincipalOpcoes();
                string entrada = Console.ReadLine();

                if (int.TryParse(entrada, out opcao))
                {
                    switch (opcao)
                    {
                        case 1:
                            await MenuAdocaoDeMascotes();
                            break;
                        case 2:
                            _view.ExibirMensagem("Esta opção estará disponível em breve! Por favor, aguarde.");
                            break;
                        case 3:
                            _view.ExibirMensagem("Obrigado por jogar! Até a próxima!");
                            break;
                        default:
                            _view.ExibirMensagem("Opção inválida. Por favor, escolha 1, 2 ou 3.");
                            break;
                    }
                }
                else
                {
                    _view.ExibirMensagem("Entrada inválida. Por favor, digite um número.");
                }
                _view.ExibirMensagem("\nPressione qualquer tecla para voltar ao menu principal...");
                Console.ReadKey();
                _view.LimparTela();
            }
        }

        private async Task MenuAdocaoDeMascotes()
        {
            _view.LimparTela();
            _view.ExibirMensagem("\n--- MENU DE ADOÇÃO DE MASCOTES ---");
            _view.ExibirMensagem("Buscando a lista inicial de Pokémons...");

            PokemonListResponse pokemonList = null;
            try
            {
                var responseLista = await _httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon/?limit=20");
                responseLista.EnsureSuccessStatusCode();
                string jsonListaPokemons = await responseLista.Content.ReadAsStringAsync();

                pokemonList = JsonSerializer.Deserialize<PokemonListResponse>(jsonListaPokemons);

                if (pokemonList != null && pokemonList.Results != null)
                {
                    _view.ExibirListaPokemons(pokemonList.Results);
                }
                else
                {
                    _view.ExibirMensagem("Não foi possível carregar a lista de Pokémons.");
                }
            }
            catch (HttpRequestException e)
            {
                _view.ExibirMensagem($"Erro ao buscar a lista de Pokémons: {e.Message}");
                return;
            }
            catch (JsonException ex)
            {
                _view.ExibirMensagem($"Erro ao processar a lista de Pokémons (JSON): {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                _view.ExibirMensagem($"Ocorreu um erro inesperado ao carregar a lista: {ex.Message}");
                return;
            }

            string escolhaPokemon = "";
            while (true)
            {
                _view.ExibirMensagem("\n-------------------------------------------------");
                _view.ExibirMensagem("Digite o NOME do Pokémon que deseja ver detalhes (ou 'voltar' para o Menu Principal): ");
                escolhaPokemon = Console.ReadLine().ToLower().Trim();

                if (escolhaPokemon == "voltar")
                {
                    _view.ExibirMensagem("Voltando ao Menu Principal...");
                    return;
                }

                Mascote mascoteEscolhido = null;
                try
                {
                    _view.ExibirMensagem($"Buscando detalhes de {escolhaPokemon}...");
                    var responseEspecifico = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{escolhaPokemon}/");
                    responseEspecifico.EnsureSuccessStatusCode();
                    string jsonPokemonEspecifico = await responseEspecifico.Content.ReadAsStringAsync();

                    mascoteEscolhido = JsonSerializer.Deserialize<Mascote>(jsonPokemonEspecifico);

                    if (mascoteEscolhido != null)
                    {
                        _view.ExibirDetalhesMascote(mascoteEscolhido);

                        _view.ExibirMensagem("\n-------------------------------------------------");
                        _view.ExibirMensagem($"Você gostaria de adotar {mascoteEscolhido.Name}? (sim/nao): ");
                        string respostaAdocao = Console.ReadLine().ToLower().Trim();

                        if (respostaAdocao == "sim")
                        {
                            _view.ExibirMensagem($"🎉 Parabéns! Você adotou o {mascoteEscolhido.Name}!");
                            return;
                        }
                        else
                        {
                            _view.ExibirMensagem($"Ok, {mascoteEscolhido.Name} não foi adotado. Você pode escolher outro.");
                        }
                    }
                    else
                    {
                        _view.ExibirMensagem($"Erro: Não foi possível obter detalhes de {escolhaPokemon}.");
                    }
                }
                catch (HttpRequestException e)
                {
                    if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _view.ExibirMensagem($"Pokémon '{escolhaPokemon}' não encontrado. Verifique a grafia.");
                    }
                    else
                    {
                        _view.ExibirMensagem($"Erro ao buscar detalhes de {escolhaPokemon} (Requisição HTTP): {e.Message}");
                    }
                }
                catch (JsonException ex)
                {
                    _view.ExibirMensagem($"Erro ao processar detalhes de {escolhaPokemon} (JSON): {ex.Message}");
                }
                catch (Exception ex)
                {
                    _view.ExibirMensagem($"Ocorreu um erro inesperado ao buscar detalhes de {escolhaPokemon}: {ex.Message}");
                }
            }
        }
    }
}
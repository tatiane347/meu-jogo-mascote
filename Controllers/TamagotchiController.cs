// Controllers/TamagotchiController.cs
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;

// >>> ESSAS DUAS LINHAS ESTÃO CORRETAS E JÁ FORAM ADICIONADAS
using AutoMapper; // Permite usar a biblioteca AutoMapper
using MeuJogoMascote.Configs; // Permite usar seu MappingProfile

using MeuJogoMascote.Models;
using MeuJogoMascote.Views;

namespace MeuJogoMascote.Controllers
{
    public class TamagotchiController
    {
        private ConsoleHandler _view;
        private static readonly HttpClient _httpClient = new HttpClient();
        private Random _random = new Random(); // Instância de Random para valores aleatórios

        // Mascote adotado que persistirá durante a sessão de jogo
        private Mascote _mascoteAdotado;
        private readonly IMapper _mapper; // >>> ESTA LINHA ESTÁ CORRETA E NO LUGAR CERTO

        public TamagotchiController(ConsoleHandler view)
        {
            _view = view;
            // >>> AS LINHAS ABAIXO FORAM MOVIDAS PARA DENTRO DO CONSTRUTOR.
            // É AQUI QUE O AUTOMAPPER DEVE SER CONFIGURADO QUANDO O CONTROLADOR É CRIADO.
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile()); // Adiciona o seu MappingProfile
            });
            _mapper = mapperConfig.CreateMapper(); // Atribui a configuração à sua ferramenta _mapper
        }

        public async Task Jogar()
        {
            _view.ExibirBoasVindas();

            int opcao = 0;
            while (opcao != 3)
            {
                // NOVIDADE: Atualiza o status do mascote antes de exibir qualquer coisa
                if (_mascoteAdotado != null)
                {
                    AtualizarStatusMascote(_mascoteAdotado); // CHAMADA DO NOVO MÉTODO
                    _view.ExibirStatusMascote(_mascoteAdotado); // Exibe o status atualizado
                }

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
                            if (_mascoteAdotado != null)
                            {
                                await MenuInteracaoMascote(); // Adicionado await aqui também
                            }
                            else
                            {
                                _view.ExibirMensagem("Você ainda não adotou um mascote! Adote um primeiro.");
                            }
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

                Mascote mascoteTemporario = null;
                try
                {
                    _view.ExibirMensagem($"Buscando detalhes de {escolhaPokemon}...");
                    var responseEspecifico = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{escolhaPokemon}/");
                    responseEspecifico.EnsureSuccessStatusCode();
                    string jsonPokemonEspecifico = await responseEspecifico.Content.ReadAsStringAsync();

                    // >>> AQUI ESTÁ A MUDANÇA MAIS IMPORTANTE PARA O AUTOMAPPER:
                    // 1. Deserializa para PokemonApiDto (o que vem da API)
                    var pokemonApiDto = JsonSerializer.Deserialize<PokemonApiDto>(jsonPokemonEspecifico);

                    if (pokemonApiDto != null)
                    {
                        // 2. Mapeia de PokemonApiDto para Mascote usando o AutoMapper
                        mascoteTemporario = _mapper.Map<Mascote>(pokemonApiDto);

                        // 3. Inicializa as necessidades do mascote com valores randômicos (não vêm da API, são do seu jogo)
                        mascoteTemporario.Alimentacao = _random.Next(5, 11);
                        mascoteTemporario.Humor = _random.Next(5, 11);
                        mascoteTemporario.Sono = _random.Next(5, 11);
                        mascoteTemporario.EstaDormindo = false;
                        mascoteTemporario.UltimaInteracao = DateTime.Now; // Registra o momento da adoção

                        _view.ExibirDetalhesMascote(mascoteTemporario);

                        _view.ExibirMensagem("\n-------------------------------------------------");
                        _view.ExibirMensagem($"Você gostaria de adotar {mascoteTemporario.Name}? (sim/nao): ");
                        string respostaAdocao = Console.ReadLine().ToLower().Trim();

                        if (respostaAdocao == "sim")
                        {
                            _mascoteAdotado = mascoteTemporario;
                            _view.ExibirMensagem($"🎉 Parabéns! Você adotou o {_mascoteAdotado.Name}!");
                            return; // Sai do MenuAdocaoDeMascotes após a adoção
                        }
                        else
                        {
                            _view.ExibirMensagem($"Ok, {mascoteTemporario.Name} não foi adotado. Você pode escolher outro.");
                        }
                    }
                    else // Se o pokemonApiDto for nulo, a desserialização falhou
                    {
                        _view.ExibirMensagem($"Erro: Não foi possível obter detalhes de {escolhaPokemon}. Verifique o nome ou a conexão.");
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

        // MÉTODO DE ATUALIZAÇÃO DO STATUS DO MASCOTE (COM DEGRADAÇÃO AJUSTADA)
        private void AtualizarStatusMascote(Mascote mascote)
        {
            TimeSpan tempoPassado = DateTime.Now - mascote.UltimaInteracao;

            int declinioAlimentacao = (int)(tempoPassado.TotalSeconds / 60);
            int declinioHumor = (int)(tempoPassado.TotalSeconds / 60);
            int declinioSono = (int)(tempoPassado.TotalSeconds / 60);

            if (declinioAlimentacao > 0)
            {
                mascote.Alimentacao = Math.Max(0, mascote.Alimentacao - declinioAlimentacao);
            }
            if (declinioHumor > 0)
            {
                mascote.Humor = Math.Max(0, mascote.Humor - declinioHumor);
            }
            if (declinioSono > 0)
            {
                if (!mascote.EstaDormindo)
                {
                    mascote.Sono = Math.Max(0, mascote.Sono - declinioSono);
                }
            }

            if (mascote.Alimentacao <= 2 || mascote.Sono <= 2)
            {
                mascote.Humor = Math.Max(0, mascote.Humor - 1);
            }

            mascote.UltimaInteracao = DateTime.Now;
        }


        private async Task MenuInteracaoMascote() // Mantido como async
        {
            if (_mascoteAdotado == null)
            {
                _view.ExibirMensagem("Você não tem um mascote para interagir.");
                return;
            }

            _view.LimparTela();
            _view.ExibirMensagem($"\n--- INTERAGIR COM {_mascoteAdotado.Name.ToUpper()} ---");

            int opcaoInteracao = 0;
            while (opcaoInteracao != 5)
            {
                AtualizarStatusMascote(_mascoteAdotado);
                _view.ExibirStatusMascote(_mascoteAdotado);

                _view.ExibirMensagem("\nEscolha uma interação:");
                _view.ExibirMensagem("1. Alimentar");
                _view.ExibirMensagem("2. Brincar");
                _view.ExibirMensagem("3. Colocar para Dormir");
                _view.ExibirMensagem("4. Acordar");
                _view.ExibirMensagem("5. Voltar ao Menu Principal");
                _view.ExibirMensagem("Sua escolha: ");

                string entrada = Console.ReadLine();
                if (int.TryParse(entrada, out opcaoInteracao))
                {
                    switch (opcaoInteracao)
                    {
                        case 1:
                            AlimentarMascote(_mascoteAdotado);
                            break;
                        case 2:
                            BrincarComMascote(_mascoteAdotado);
                            break;
                        case 3:
                            ColocarMascoteParaDormir(_mascoteAdotado);
                            break;
                        case 4:
                            AcordarMascote(_mascoteAdotado);
                            break;
                        case 5:
                            _view.ExibirMensagem("Voltando ao menu principal...");
                            break;
                        default:
                            _view.ExibirMensagem("Opção inválida. Por favor, escolha entre 1 e 5.");
                            break;
                    }
                }
                else
                {
                    _view.ExibirMensagem("Entrada inválida. Por favor, digite um número.");
                }

                if (opcaoInteracao != 5)
                {
                    _view.ExibirMensagem("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    _view.LimparTela();
                }
            }
            await Task.CompletedTask;
        }

        private void AlimentarMascote(Mascote mascote)
        {
            if (mascote.EstaDormindo)
            {
                _view.ExibirMensagem($"{mascote.Name} está dormindo e não pode ser alimentado agora!");
                return;
            }
            if (mascote.Alimentacao >= 10)
            {
                _view.ExibirMensagem($"{mascote.Name} já está cheio! Não precisa de mais comida.");
                return;
            }

            mascote.Alimentacao = Math.Min(10, mascote.Alimentacao + 3);
            mascote.Humor = Math.Min(10, mascote.Humor + 1);
            mascote.UltimaInteracao = DateTime.Now;
            _view.ExibirMensagem($"Você alimentou {mascote.Name}. Ele agora está com {mascote.Alimentacao}/10 de alimentação!");
        }

        private void BrincarComMascote(Mascote mascote)
        {
            if (mascote.EstaDormindo)
            {
                _view.ExibirMensagem($"{mascote.Name} está dormindo e não pode brincar agora!");
                return;
            }
            if (mascote.Alimentacao <= 2)
            {
                _view.ExibirMensagem($"{mascote.Name} está muito faminto para brincar agora. Alimente-o primeiro!");
                return;
            }
            if (mascote.Humor >= 10)
            {
                _view.ExibirMensagem($"{mascote.Name} já está radiante de felicidade! Não precisa de mais brincadeiras.");
                return;
            }

            mascote.Alimentacao = Math.Max(0, mascote.Alimentacao - 1);
            mascote.Humor = Math.Min(10, mascote.Humor + 2);
            mascote.Sono = Math.Max(0, mascote.Sono - 1);
            mascote.UltimaInteracao = DateTime.Now;
            _view.ExibirMensagem($"Você brincou com {mascote.Name}. O humor dele está em {mascote.Humor}/10 e alimentação em {mascote.Alimentacao}/10!");
        }

        private void ColocarMascoteParaDormir(Mascote mascote)
        {
            if (mascote.EstaDormindo)
            {
                _view.ExibirMensagem($"{mascote.Name} já está dormindo!");
                return;
            }
            if (mascote.Sono >= 10)
            {
                _view.ExibirMensagem($"{mascote.Name} não está com sono. Ele está bem descansado!");
                return;
            }

            mascote.EstaDormindo = true;
            mascote.Sono = Math.Min(10, mascote.Sono + 5);
            mascote.UltimaInteracao = DateTime.Now;
            _view.ExibirMensagem($"Você colocou {mascote.Name} para dormir. Zzzzz...");
            _view.ExibirMensagem($"O sono de {mascote.Name} agora está em {mascote.Sono}/10.");
        }

        private void AcordarMascote(Mascote mascote)
        {
            if (!mascote.EstaDormindo)
            {
                _view.ExibirMensagem($"{mascote.Name} já está acordado!");
                return;
            }

            mascote.EstaDormindo = false;
            mascote.UltimaInteracao = DateTime.Now;
            _view.ExibirMensagem($"{mascote.Name} acordou! Bom dia!");
        }
    }
}
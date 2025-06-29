// Controllers/TamagotchiController.cs
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq; // Para usar .Any() em alguns cenários
using System.Collections.Generic;

using AutoMapper;
using MeuJogoMascote.Configs; // Permite usar seu MappingProfile
using MeuJogoMascote.Models;
using MeuJogoMascote.Views;

namespace MeuJogoMascote.Controllers
{
    public class TamagotchiController
    {
        private ConsoleHandler _view;
        private static readonly HttpClient _httpClient = new HttpClient(); // HttpClient para chamadas à API
        private Random _random = new Random(); // Instância de Random para valores aleatórios

        private Mascote _mascoteAdotado; // Mascote adotado que persistirá durante a sessão de jogo
        private readonly IMapper _mapper; // Instância do AutoMapper

        // Construtor: Injetamos a view e configuramos o AutoMapper
        public TamagotchiController(ConsoleHandler view)
        {
            _view = view;
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile()); // Adiciona o seu MappingProfile
            });
            _mapper = mapperConfig.CreateMapper(); // Atribui a configuração à sua ferramenta _mapper
        }

        // Método principal para iniciar o jogo
        public async Task IniciarJogo() // Renomeado de Jogar() para IniciarJogo() para clareza
        {
            _view.ExibirBoasVindas();

            bool sairDoJogo = false;

            // Tratativa de exceções abrangente para o loop principal do jogo
            try
            {
                while (!sairDoJogo)
                {
                    // NOVIDADE: Atualiza o status do mascote a cada "turno" do menu
                    if (_mascoteAdotado != null)
                    {
                        AtualizarStatusMascote(_mascoteAdotado); // CHAMADA DO MÉTODO DE ATUALIZAÇÃO DE STATUS
                        _view.ExibirStatusMascote(_mascoteAdotado); // Exibe o status atualizado
                    }

                    _view.ExibirMenuPrincipalOpcoes();
                    string entrada = Console.ReadLine();
                    int opcao;

                    // Validação robusta de entrada para o menu principal
                    if (int.TryParse(entrada, out opcao))
                    {
                        switch (opcao)
                        {
                            case 1: // Adoção de Mascotes
                                await MenuAdocaoDeMascotes();
                                break;
                            case 2: // Interagir com Mascote
                                if (_mascoteAdotado != null)
                                {
                                    await MenuInteracaoMascote(); // Aguarda a conclusão da interação
                                }
                                else
                                {
                                    _view.ExibirMensagem("Você ainda não adotou um mascote! Adote um primeiro.");
                                }
                                break;
                            case 3: // Sair do Jogo
                                _view.ExibirMensagem("Obrigado por jogar! Até a próxima!");
                                sairDoJogo = true;
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

                    if (!sairDoJogo) // Só pausa e limpa a tela se o jogo não estiver saindo
                    {
                        _view.ExibirMensagem("\nPressione qualquer tecla para continuar...");
                        Console.ReadKey();
                        _view.LimparTela();
                    }
                }
            }
            catch (Exception ex) // Captura qualquer exceção não tratada no loop principal
            {
                _view.ExibirMensagem($"Ocorreu um erro crítico no jogo: {ex.Message}");
                _view.ExibirMensagem("Por favor, reinicie a aplicação.");
                _view.ExibirMensagem("Pressione qualquer tecla para sair...");
                Console.ReadKey();
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
                responseLista.EnsureSuccessStatusCode(); // Lança HttpRequestException para status de erro (4xx, 5xx)
                string jsonListaPokemons = await responseLista.Content.ReadAsStringAsync();

                pokemonList = JsonSerializer.Deserialize<PokemonListResponse>(jsonListaPokemons);

                if (pokemonList != null && pokemonList.Results != null && pokemonList.Results.Any())
                {
                    _view.ExibirListaPokemons(pokemonList.Results);
                }
                else
                {
                    // Cenário de API indisponível / sem retorno válido
                    _view.ExibirMensagem("Não foi possível carregar a lista de Pokémons. A API pode estar indisponível ou vazia.");
                    return; // Retorna para o menu principal
                }
            }
            catch (HttpRequestException e)
            {
                _view.ExibirMensagem($"Erro de conexão ou API indisponível: {e.Message}. Verifique sua internet ou tente mais tarde.");
                // Opcional: Desserializar a mensagem de erro se a API fornecer um JSON de erro
                if (e.StatusCode.HasValue)
                {
                    _view.ExibirMensagem($"Status HTTP: {(int)e.StatusCode.Value}");
                    // Se você souber que a API retorna um JSON de erro específico, você pode tentar desserializá-lo aqui
                    // Ex: string errorContent = await e.Content.ReadAsStringAsync();
                    //     var apiError = JsonSerializer.Deserialize<ApiErrorMessage>(errorContent);
                    //     _view.ExibirMensagem($"Detalhes do erro da API: {apiError.Message}");
                }
                return;
            }
            catch (JsonException ex)
            {
                _view.ExibirMensagem($"Erro ao processar a lista de Pokémons (JSON inválido): {ex.Message}");
                _view.ExibirMensagem("Isso pode indicar um problema com os dados retornados pela API.");
                return;
            }
            catch (Exception ex)
            {
                _view.ExibirMensagem($"Ocorreu um erro inesperado ao carregar a lista de Pokémons: {ex.Message}");
                return;
            }

            string escolhaPokemon = "";
            while (true) // Loop para garantir que o usuário escolha um Pokémon válido ou decida voltar
            {
                _view.ExibirMensagem("\n-------------------------------------------------");
                _view.ExibirMensagem("Digite o NOME do Pokémon que deseja ver detalhes (ou 'voltar' para o Menu Principal): ");
                escolhaPokemon = Console.ReadLine().ToLower().Trim();

                if (string.IsNullOrWhiteSpace(escolhaPokemon)) // Validação para entrada vazia
                {
                    _view.ExibirMensagem("Nome do Pokémon não pode ser vazio. Digite um nome ou 'voltar'.");
                    continue; // Pede a entrada novamente
                }

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

                    if (!responseEspecifico.IsSuccessStatusCode) // Captura status de erro específicos (404, 500, etc.)
                    {
                        if (responseEspecifico.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            _view.ExibirMensagem($"Pokémon '{escolhaPokemon}' não encontrado. Verifique a grafia.");
                        }
                        else // Outros erros HTTP (500, etc.)
                        {
                            string errorContent = await responseEspecifico.Content.ReadAsStringAsync();
                            _view.ExibirMensagem($"Erro ao buscar detalhes de {escolhaPokemon}: Status {responseEspecifico.StatusCode} - {errorContent}");
                        }
                        continue; // Permite ao usuário tentar outro nome ou voltar
                    }

                    string jsonPokemonEspecifico = await responseEspecifico.Content.ReadAsStringAsync();

                    var pokemonApiDto = JsonSerializer.Deserialize<PokemonApiDto>(jsonPokemonEspecifico);

                    if (pokemonApiDto != null)
                    {
                        mascoteTemporario = _mapper.Map<Mascote>(pokemonApiDto);

                        // Inicializa as necessidades do mascote
                        mascoteTemporario.Alimentacao = _random.Next(5, 11); // Valores iniciais aleatórios
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
                    else // Se o pokemonApiDto for nulo, a desserialização falhou (mas o status foi 200 OK)
                    {
                        _view.ExibirMensagem($"Erro: Não foi possível processar os dados de {escolhaPokemon}. Dados inválidos da API.");
                    }
                }
                catch (JsonException ex)
                {
                    _view.ExibirMensagem($"Erro ao processar detalhes de {escolhaPokemon} (JSON inválido): {ex.Message}");
                }
                catch (Exception ex) // Catch para qualquer outra exceção inesperada
                {
                    _view.ExibirMensagem($"Ocorreu um erro inesperado ao buscar detalhes de {escolhaPokemon}: {ex.Message}");
                }
            }
        }

        // Método de atualização do status do mascote com degradação
        private void AtualizarStatusMascote(Mascote mascote)
        {
            TimeSpan tempoPassado = DateTime.Now - mascote.UltimaInteracao;

            // Calcula o quanto as necessidades devem diminuir.
            // Para testes rápidos, pode ser TotalSeconds. Para um jogo "real", TotalMinutes ou TotalHours.
            // Ajustei para TotalMinutes para não cair tão rápido durante testes.
            int minutosPassados = (int)tempoPassado.TotalMinutes;

            if (minutosPassados > 0)
            {
                // Para cada minuto que passou, diminui a alimentação, humor, sono
                mascote.Alimentacao = Math.Max(0, mascote.Alimentacao - minutosPassados);
                mascote.Humor = Math.Max(0, mascote.Humor - (mascote.EstaDormindo ? 0 : minutosPassados)); // Humor não cai dormindo
                mascote.Sono = Math.Max(0, mascote.Sono - (mascote.EstaDormindo ? 0 : minutosPassados)); // Sono não cai dormindo

                // Se o mascote estiver dormindo, o sono pode até aumentar lentamente (opcional)
                if (mascote.EstaDormindo)
                {
                    mascote.Sono = Math.Min(10, mascote.Sono + (minutosPassados / 2)); // Aumenta sono pela metade do tempo
                }

                // Se as necessidades básicas caem muito, o humor também cai mais
                if (mascote.Alimentacao <= 2 || mascote.Sono <= 2)
                {
                    mascote.Humor = Math.Max(0, mascote.Humor - 1);
                }

                // Se o mascote está muito mal, ele pode ficar automaticamente dormindo ou doente (extensão futura)
                if (mascote.Alimentacao <= 0 || mascote.Humor <= 0 || mascote.Sono <= 0)
                {
                    _view.ExibirMensagem($"Cuidado! {mascote.Name} não está se sentindo bem!");
                    // Aqui você poderia adicionar lógica para o mascote "morrer" ou ficar doente
                }
            }

            mascote.UltimaInteracao = DateTime.Now; // Atualiza a última interação para o tempo atual
        }


        // Método para gerenciar as interações com o mascote
        private async Task MenuInteracaoMascote()
        {
            if (_mascoteAdotado == null)
            {
                _view.ExibirMensagem("Você não tem um mascote para interagir.");
                await Task.CompletedTask; // Retorna uma tarefa completa se não houver mascote
                return;
            }

            _view.LimparTela();
            _view.ExibirMensagem($"\n--- INTERAGIR COM {_mascoteAdotado.Name.ToUpper()} ---");

            int opcaoInteracao = 0;
            while (opcaoInteracao != 5) // Loop continua até o usuário escolher 'Voltar' (opção 5)
            {
                AtualizarStatusMascote(_mascoteAdotado); // Atualiza status antes de exibir e interagir
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

                if (opcaoInteracao != 5) // Só pausa e limpa a tela se não estiver voltando
                {
                    _view.ExibirMensagem("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    _view.LimparTela();
                }
            }
            // Não precisa de Task.CompletedTask aqui, pois o método já é async e o return/break no loop o finaliza.
        }

        // Métodos de Interação (já estavam bem feitos!)
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
            mascote.Sono = Math.Max(0, mascote.Sono - 1); // Brincando cansa
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
            // Se o mascote estiver muito acordado, ele pode não querer dormir ainda
            if (mascote.Sono >= 8) // Ajustei o limiar para 8, antes era 10
            {
                _view.ExibirMensagem($"{mascote.Name} não está com sono. Ele está bem descansado!");
                return;
            }

            mascote.EstaDormindo = true;
            mascote.Sono = Math.Min(10, mascote.Sono + 5); // Aumenta bastante o sono ao dormir
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
            // O mascote só acorda se tiver sono suficiente ou se for "forçado" a acordar
            // Aumentar o humor ligeiramente ao acordar de um bom sono
            mascote.EstaDormindo = false;
            mascote.Humor = Math.Min(10, mascote.Humor + 1); // Acorda com um pouco mais de humor
            mascote.UltimaInteracao = DateTime.Now;
            _view.ExibirMensagem($"{mascote.Name} acordou! Bom dia!");
        }
    }
}
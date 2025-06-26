// main.cs
using System;
using System.Threading.Tasks;

// Ajuste estes 'using's para apontar para as suas Views e Controllers corretas!
using MeuJogoMascote.Views;
using MeuJogoMascote.Controllers;

namespace MeuJogoMascote // Este é o namespace raiz do seu projeto
{
    public class Program // A classe Program deve ser 'public'
    {
        static async Task Main(string[] args)
        {
            // 1. Cria uma instância da nossa View (ConsoleHandler)
            ConsoleHandler consoleView = new ConsoleHandler();

            // 2. Cria uma instância da nossa Controller (TamagotchiController)
            // e passa a View para ela (injeção de dependência)
            TamagotchiController gameController = new TamagotchiController(consoleView);

            // 3. Inicia o jogo através da Controller
            await gameController.Jogar();
        }
    }
}
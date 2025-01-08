using System;
using RotaDeViagem.Services;

namespace RotaDeViagem.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Por favor, forneça o arquivo de rotas 'rotas.csv' como argumento.");
                Console.WriteLine("Exemplo: dotnet run --project RotaDeViagem.ConsoleApp/RotaDeViagem.ConsoleApp.csproj rotas.csv");
                return;
            }

            string filePath = args[0];
            RouteService routeService = new(filePath);

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Digite a rota (DE-PARA). 'list' para listar destinos disponíveis. 'sair / exit' para encerrar: ");
                string input = Console.ReadLine();
                
                input = input.ToUpper();

                Console.ResetColor();

                if (input?.ToLower() == "sair" || input?.ToLower() == "exit")
                {
                    break;
                }

                if (input?.ToLower() == "list")
                {
                    var routesList = routeService.ListRoutesCsv();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Destinos disponíveis: {routesList}");
                    Console.ResetColor();
                    continue;
                }

                var parts = input?.Split('-');
                if (parts?.Length != 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Formato inválido. Use DE-PARA. Exemplo: GRU-CDG");
                    Console.ResetColor();
                    continue;
                }

                string origin = parts[0].Trim();
                string destination = parts[1].Trim();

                var result = routeService.FindCheapestRoute(origin, destination);
                var finalResult = result?.Path;
                var cost = result?.Cost;
                if (result != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Melhor Rota: '{finalResult}' ao custo de ${cost}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Rota não encontrada. Verifique se digitou corretamente utilizando o comando 'list'");
                    Console.ResetColor();
                }
            }
        }
    }
}
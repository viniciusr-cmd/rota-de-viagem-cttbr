using System;
using System.Collections.Generic;
using System.IO;
using RotaDeViagem.Services.Models;

namespace RotaDeViagem.Services.Utils
{
    public static class CsvHelper
    {
        public static List<Route> ReadRoutesFromCsv(string filePath)
        {
            var routes = new List<Route>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("CSV route file not found.", filePath);
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length == 3 &&
                    !string.IsNullOrWhiteSpace(parts[0]) &&
                    !string.IsNullOrWhiteSpace(parts[1]) &&
                    decimal.TryParse(parts[2], out var cost))
                {
                    var route = new Route(parts[0].Trim(), parts[1].Trim(), cost);
                    routes.Add(route);
                }
            }

            return routes;
        }

        public static void WriteRouteToCsv(string filePath, Route route)
        {
            var line = $"{route.Origin},{route.Destination},{route.Cost}";
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
    }
}
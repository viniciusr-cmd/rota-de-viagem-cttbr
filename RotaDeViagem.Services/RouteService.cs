using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RotaDeViagem.Services.Models;
using RotaDeViagem.Services.Utils;

namespace RotaDeViagem.Services
{
    public class RouteService
    {
        private readonly List<Route> _routes;
        private readonly Dictionary<string, List<(string Destination, decimal Cost)>> graph;

        public RouteService(string filePath)
        {
            _routes = CsvHelper.ReadRoutesFromCsv(filePath);
            graph = BuildGraph(_routes);
        }

        private static Dictionary<string, List<(string Destination, decimal Cost)>> BuildGraph(List<Route> routes)
        {
            var graph = new Dictionary<string, List<(string Destination, decimal Cost)>>();

            foreach (var route in routes)
            {
                if (!graph.TryGetValue(route.Origin, out List<(string Destination, decimal Cost)> value))
                {
                    value = new List<(string Destination, decimal Cost)>();
                    graph[route.Origin] = value;
                }

                value.Add((route.Destination, route.Cost));
            }

            return graph;
        }

        public void AddRoute(Route route)
        {
            _routes.Add(route);
            CsvHelper.WriteRouteToCsv("../rotas.csv", route);
            if (!graph.TryGetValue(route.Origin, out List<(string Destination, decimal Cost)> value))
            {
                value = [];
                graph[route.Origin] = value;
            }

            value.Add((route.Destination, route.Cost));
        }

        public (string Path, decimal Cost)? FindCheapestRoute(string origin, string destination)
        {
            var costs = new Dictionary<string, decimal>();
            var parents = new Dictionary<string, string>();
            var processed = new HashSet<string>();

            foreach (var route in _routes)
            {
                costs[route.Origin] = decimal.MaxValue;
                costs[route.Destination] = decimal.MaxValue;
            }
            costs[origin] = 0;

            string FindLowestCostNode()
            {
                var lowestCost = decimal.MaxValue;
                string lowestCostNode = null;

                foreach (var node in costs)
                {
                    var cost = node.Value;
                    if (cost < lowestCost && !processed.Contains(node.Key))
                    {
                        lowestCost = cost;
                        lowestCostNode = node.Key;
                    }
                }
                return lowestCostNode;
            }

            var node = FindLowestCostNode();
            while (node != null)
            {
                var cost = costs[node];
                var neighbors = graph.TryGetValue(node, out List<(string Destination, decimal Cost)> value) ? value : [];

                foreach (var (Destination, Cost) in neighbors)
                {
                    var newCost = cost + Cost;
                    if (newCost < costs[Destination])
                    {
                        costs[Destination] = newCost;
                        parents[Destination] = node;
                    }
                }
                processed.Add(node);
                node = FindLowestCostNode();
            }

            if (!parents.ContainsKey(destination))
            {
                return null;
            }

            var path = new List<string>();
            var currentNode = destination;
            while (currentNode != null && parents.ContainsKey(currentNode))
            {
                path.Add(currentNode);
                currentNode = parents[currentNode];
            }
            path.Add(origin);
            path.Reverse();

            return (string.Join(" -> ", path), costs[destination]);
        }
        public string ListRoutesCsv()
        {
            var uniqueAcronyms = new HashSet<string>();

            foreach (var route in _routes)
            {
                uniqueAcronyms.Add(route.Origin);
                uniqueAcronyms.Add(route.Destination);
            }

            return string.Join(" - ", uniqueAcronyms);
        }
    }
}
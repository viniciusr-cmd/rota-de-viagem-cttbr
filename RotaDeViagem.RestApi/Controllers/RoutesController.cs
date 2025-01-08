using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using RotaDeViagem.Services;
using RotaDeViagem.Services.Models;

namespace RotaDeViagem.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly RouteService _routeService;
        private readonly IHttpClientFactory _httpClientFactory;

        public RoutesController(RouteService routeService, IHttpClientFactory httpClientFactory)
        {
            _routeService = routeService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("best-route")]
        public ActionResult<string> GetBestRoute(string origin, string destination)
        {
            origin = origin?.ToUpper();
            destination = destination?.ToUpper();

            if (origin == destination)
            {
                return BadRequest(new { message = "Origem e destino não podem ser iguais.", origin, destination });
            }
            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
            {
                return BadRequest(new { message = "Origem e destino são obrigatórios.", origin, destination });
            }
            var result = _routeService.FindCheapestRoute(origin, destination);
            var bestRoute = result?.Path;
            var cost = result?.Cost;
            if (bestRoute == null)
            {
                return NotFound(new { message = "A rota especificada não pôde ser encontrada", origin, destination });
            }
            return Ok(new { message = "Rota encontrada com sucesso", origin, destination, bestRoute, cost });
        }

        [HttpPost("add-route")]
        public ActionResult AddRoute([FromBody] Route route)
        {
            route.Origin = route.Origin?.ToUpper();
            route.Destination = route.Destination?.ToUpper();

            if (route.Origin == route.Destination)
            {
                return BadRequest(new { message = "Origem e destino não podem ser iguais.", origin = route.Origin, destination = route.Destination });
            }
                if (route == null || string.IsNullOrEmpty(route.Origin) || string.IsNullOrEmpty(route.Destination) || route.Cost <= 0)
            {
                return BadRequest(new { message = "Dados inválidos.", description = "Certifique -se de que a rota tenha origem, destino e custo válidos maiores que zero." });
            }

            _routeService.AddRoute(route);
            return CreatedAtAction(nameof(GetBestRoute), new { origin = route.Origin, destination = route.Destination }, new { message = "Rota criada com sucesso.", route });
        }

        [HttpGet("list-routes")]
        public ActionResult GetRoutes()
        {
            var routes = _routeService.ListRoutesCsv();
            if (string.IsNullOrEmpty(routes))
            {
                return NotFound(new { message = "Rotas não encontradas." });
            }
            return Ok(new { message = "Rotas disponíveis", routes });
        }
    }
}
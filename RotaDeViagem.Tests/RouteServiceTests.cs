using Xunit;
using RotaDeViagem.Services;

namespace RotaDeViagem.Tests
{
    public class RouteServiceTests
    {
        private readonly RouteService _routeService;

        public RouteServiceTests()
        {
            _routeService = new RouteService("../../../../rotas.csv");
        }

        [Fact]
        public void LoadRoutes_ShouldLoadRoutesFromCsv()
        {
            var routes = _routeService.ListRoutesCsv();
            Assert.NotEmpty(routes);
            Assert.Equal("GRU - BRC - SCL - CDG - ORL", routes);
        }
        [Fact]
        public void GetBestRoute_ShouldReturnCorrectRoute()
        {
            var bestRoute = _routeService.FindCheapestRoute("GRU", "CDG");
            Assert.NotNull(bestRoute);
            Assert.Equal("GRU -> BRC -> SCL -> ORL -> CDG", bestRoute?.Path);
            Assert.Equal(40, bestRoute?.Cost);
        }

        [Fact]
        public void GetBestRoute_ShouldReturnNullForInvalidRoute()
        {
            var bestRoute = _routeService.FindCheapestRoute("INVALID", "CDG");
            Assert.Null(bestRoute);
        }
    }
}
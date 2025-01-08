namespace RotaDeViagem.Services.Models
{
    public class Route
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public decimal Cost { get; set; }

        public Route(string origin, string destination, decimal cost)
        {
            Origin = origin;
            Destination = destination;
            Cost = cost;
        }
    }
}
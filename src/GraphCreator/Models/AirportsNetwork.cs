namespace GraphCreator.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class AirportsNetwork
    {
        public List<Airport> Airports { get; } = new List<Airport>();
        public List<Country> Countries { get; } = new List<Country>();
        public List<Continent> Continents { get; } = new List<Continent>();
        public List<Route> Routes { get; } = new List<Route>();
        public List<Edge> Relations { get; } = new List<Edge>();


        public void CleanRelations()
        {
            var entitiesIds = this.Airports.Select(a => a.Id).ToList();
            entitiesIds.AddRange(this.Countries.Select(c => c.Id).ToList());
            entitiesIds.AddRange(this.Continents.Select(c => c.Id).ToList());

            this.Relations.RemoveAll(x => !entitiesIds.Contains(x.From) || !entitiesIds.Contains(x.To));
        }
    }
}

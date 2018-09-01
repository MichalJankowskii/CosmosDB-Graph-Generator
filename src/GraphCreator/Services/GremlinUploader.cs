namespace GraphCreator.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using ExtensionMethods;
    using Gremlin.Net.Driver;
    using Gremlin.Net.Structure.IO.GraphSON;
    using Models;

    public class GremlinUploader
    {
        private readonly string hostName;
        private readonly int port;
        private readonly string authKey;
        private readonly string databaseName;
        private readonly string collectionName;

        public GremlinUploader(string hostName, int port, string authKey, string databaseName, string collectionName)
        {
            this.hostName = hostName;
            this.port = port;
            this.authKey = authKey;
            this.databaseName = databaseName;
            this.collectionName = collectionName;
        }

        public void UploadAirportsNetwork(AirportsNetwork network)
        {
            var gremlinServer = new GremlinServer(this.hostName, port, enableSsl: true,
                username: "/dbs/" + this.databaseName + "/colls/" + this.collectionName, password: authKey);
            using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType))
            {

                gremlinClient.SubmitAsync<dynamic>("g.V().drop()").Wait();

                Console.WriteLine("Uploading airports");
                foreach (Airport airport in network.Airports)
                {
                    var query = CreateAirportQuery(airport);
                    gremlinClient.SubmitAsync<dynamic>(query).Wait();
                }

                Console.WriteLine("Uploading countries");
                foreach (Country country in network.Countries)
                {
                    var query = CreateCountryQuery(country);
                    gremlinClient.SubmitAsync<dynamic>(query).Wait();
                }

                Console.WriteLine("Uploading continents");
                foreach (Continent continent in network.Continents)
                {
                    var query = CreateContinentQuery(continent);
                    gremlinClient.SubmitAsync<dynamic>(query).Wait();
                }

                Console.WriteLine("Uploading relations");
                foreach (Edge relation in network.Relations)
                {
                    var query = CreateContainsQuery(relation, network.Continents, network.Countries, network.Airports);
                    gremlinClient.SubmitAsync<dynamic>(query).Wait();
                }

                Console.WriteLine("Uploading routes");
                foreach (Route route in network.Routes)
                {
                    var query = CreateRouteQuery(route, network.Airports);
                    gremlinClient.SubmitAsync<dynamic>(query).Wait();
                }
            }
        }

        private static string CreateAirportQuery(Airport airport)
        {
            return $"g.addV('{airport.Label}')" +
                   $".property('id', '{airport.Code}')" +
                   $".property('Code', '{airport.Code}')" +
                   $".property('ICAO', '{airport.ICAO}')" +
                   $".property('City', '{airport.City.EnsureOnlyLetterDigitOrWhiteSpace()}')" +
                   $".property('Description', '{airport.Description.EnsureOnlyLetterDigitOrWhiteSpace()}')" +
                   $".property('Region', '{airport.Region}')" +
                   $".property('NumberOfRunways', {airport.NumberOfRunways})" +
                   $".property('DistanceOfLongestRunWay', {airport.DistanceOfLongestRunWay})" +
                   $".property('Elevation', {airport.Elevation})" +
                   $".property('Country', '{airport.Country}')" +
                   $".property('Lat', {airport.Lat.ToString(CultureInfo.InvariantCulture)})" +
                   $".property('Lon', {airport.Lon.ToString(CultureInfo.InvariantCulture)})";
        }

        private static string CreateCountryQuery(Country country)
        {
            return $"g.addV('{country.Label}')" +
                   $".property('id', '{country.Name.EnsureOnlyLetterDigitOrWhiteSpace()}')" +
                   $".property('Code', '{country.Code}')" +
                   $".property('Name', '{country.Name.EnsureOnlyLetterDigitOrWhiteSpace()}')";
        }

        private string CreateContinentQuery(Continent continent)
        {
            return $"g.addV('{continent.Label}')" +
                   $".property('id', '{continent.Name.EnsureOnlyLetterDigitOrWhiteSpace()}')" +
                   $".property('Code', '{continent.Code}')" +
                   $".property('Name', '{continent.Name.EnsureOnlyLetterDigitOrWhiteSpace()}')";
        }

        private string CreateRouteQuery(Route route, IList<Airport> airports)
        {
            var fromCode = airports.First(airport => airport.Id == route.From).Code;
            var toCode = airports.First(airport => airport.Id == route.To).Code;
            return $"g.V('{fromCode}').addE('{route.Label}').property('Distance', {route.Distance}).to(g.V('{toCode}'))";
        }

        private string CreateContainsQuery(Edge edge, IEnumerable<Continent> continents, IEnumerable<Country> countries, IEnumerable<Airport> airports)
        {
            string fromCode = countries.FirstOrDefault(country => country.Id == edge.From)?.Name;
            if (string.IsNullOrEmpty(fromCode))
            {
                fromCode = continents.First(continent => continent.Id == edge.From).Name;
            }

            string toCode = airports.First(airport => airport.Id == edge.To).Code;
            return $"g.V('{fromCode.EnsureOnlyLetterDigitOrWhiteSpace()}').addE('{edge.Label}').to(g.V('{toCode.EnsureOnlyLetterDigitOrWhiteSpace()}'))";
        }
    }
}
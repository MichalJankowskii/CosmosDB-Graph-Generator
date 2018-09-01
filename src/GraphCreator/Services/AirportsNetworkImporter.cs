namespace GraphCreator.Services
{
    using System.Globalization;
    using System.Xml;
    using Models;

    public class AirportsNetworkImporter
    {
        private AirportsNetworkImporter()
        {
        }

        public static AirportsNetworkImporter Instance { get; } = new AirportsNetworkImporter();

        public AirportsNetwork Import(NetworkSize size)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (size == NetworkSize.Normal)
            {
                xmlDoc.Load("SampleData\\air-routes.graphml");
            }
            else
            {
                xmlDoc.Load("SampleData\\air-routes-small.graphml");
            }

            var airportNetwork = new AirportsNetwork();

            foreach (XmlNode node in xmlDoc.GetElementsByTagName("node"))
            {
                switch (node.ChildNodes[0].InnerText)
                {
                    case "airport":
                        airportNetwork.Airports.Add(ParseAirport(node));
                        break;

                    case "country":
                        airportNetwork.Countries.Add(ParseCountry(node));
                        break;

                    case "continent":
                        airportNetwork.Continents.Add(ParseContinent(node));
                        break;
                }
            }

            foreach (XmlNode node in xmlDoc.GetElementsByTagName("edge"))
            {
                switch (node.ChildNodes[0].InnerText)
                {
                    case "route":
                        airportNetwork.Routes.Add(ParseRoute(node));
                        break;

                    case "contains":
                        airportNetwork.Relations.Add(ParseContain(node));
                        break;
                }
            }

            if (size == NetworkSize.Reduced)
            {
                airportNetwork.CleanRelations();
            }

            return airportNetwork;
        }

        private static Airport ParseAirport(XmlNode node)
        {
            return new Airport
            {
                Id = int.Parse(node.Attributes[0].Value),
                Label = "airport",
                Code = node.ChildNodes[2].InnerText,
                ICAO = node.ChildNodes[3].InnerText,
                City = node.ChildNodes[4].InnerText,
                Description = node.ChildNodes[5].InnerText,
                Region = node.ChildNodes[6].InnerText,
                NumberOfRunways = int.Parse(node.ChildNodes[7].InnerText),
                DistanceOfLongestRunWay = int.Parse(node.ChildNodes[8].InnerText),
                Elevation = int.Parse(node.ChildNodes[9].InnerText),
                Country = node.ChildNodes[10].InnerText,
                Lat = double.Parse(node.ChildNodes[11].InnerText, CultureInfo.InvariantCulture),
                Lon = double.Parse(node.ChildNodes[12].InnerText, CultureInfo.InvariantCulture)
            };
        }

        private static Country ParseCountry(XmlNode node)
        {
            return new Country
            {
                Id = int.Parse(node.Attributes[0].Value),
                Label = "country",
                Code = node.ChildNodes[1].InnerText,
                Name = node.ChildNodes[2].InnerText
            };
        }

        private static Continent ParseContinent(XmlNode node)
        {
            return new Continent
            {
                Id = int.Parse(node.Attributes[0].Value),
                Label = "continent",
                Code = node.ChildNodes[1].InnerText,
                Name = node.ChildNodes[2].InnerText
            };
        }

        private static Edge ParseContain(XmlNode node)
        {
            return new Edge
            {
                Id = int.Parse(node.Attributes[0].Value),
                Label = "contains",
                From = int.Parse(node.Attributes[1].Value),
                To = int.Parse(node.Attributes[2].Value)
            };
        }

        private static Route ParseRoute(XmlNode node)
        {
            return new Route
            {
                Id = int.Parse(node.Attributes[0].Value),
                Label = "route",
                From = int.Parse(node.Attributes[1].Value),
                To = int.Parse(node.Attributes[2].Value),
                Distance = int.Parse(node.ChildNodes[1].InnerText),
            };
        }
    }
}

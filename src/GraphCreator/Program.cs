namespace GraphCreator
{
    using System;
    using Services;

    class Program
    {
        private static readonly string Hostname = "ENTER HOST NAME";
        private static readonly int Port = 443;
        private static readonly string AuthKey = "ENTER AUTH KEY";
        private static readonly string Database = "TheWorld";
        private static readonly string Collection = "AirportsNetwork";


        static void Main(string[] args)
        {
            Console.WriteLine("Import started");
            var airportsNetwork = AirportsNetworkImporter.Instance.Import(NetworkSize.Reduced);
            GremlinUploader uploader = new GremlinUploader(Hostname, Port, AuthKey, Database, Collection);
            uploader.UploadAirportsNetwork(airportsNetwork);
            Console.WriteLine("Import finished");
            Console.ReadLine();
        }
    }
}

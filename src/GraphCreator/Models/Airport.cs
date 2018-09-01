namespace GraphCreator.Models
{
    public class Airport : Entity
    {
        public string Code { get; set; }
        public string ICAO { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public int NumberOfRunways{ get; set; }
        public int DistanceOfLongestRunWay { get; set; }
        public int Elevation { get; set; }
        public string Country { get; set; }
        public double Lat{ get; set; }
        public double Lon { get; set; }
    }
}

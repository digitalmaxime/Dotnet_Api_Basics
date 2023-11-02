using System.ComponentModel.DataAnnotations;

namespace IOptions_Pattern_Demo;

public class CityOption
{
    public const string cityOption = "CityOption";
    
    public City City1 { get; set; }
    public City City2 { get; set; }
    public City City3 { get; set; }

    public class City {
        public string Name { get; set; }
        public string Population { get; set; }
    }
}
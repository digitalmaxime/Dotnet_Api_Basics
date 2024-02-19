using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace IOptions_Pattern_Demo;

public class CityOption : IEnumerable<CityOption.City>
{
    public const string cityOption = "CityOption";
    
    public City City1 { get; set; }
    public City City2 { get; set; }
    public City City3 { get; set; }

    public class City {
        public string Name { get; set; }
        public string Population { get; set; }
    }

    public IEnumerator<City> GetEnumerator()
    {
        yield return City2;
        yield return City3;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
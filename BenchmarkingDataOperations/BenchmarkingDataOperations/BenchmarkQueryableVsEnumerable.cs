using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchmarkingDataOperations;

// [SimpleJob(RuntimeMoniker.Net80, baseline: true)]
[SimpleJob(RuntimeMoniker.Net90)]
[HideColumns("Job", "Median", "Error", "StdDev")]
public class BenchmarkQueryableVsEnumerable
{
    [Benchmark]
    public List<Person> Dunno()
    {
        var res = new List<Person>();
        var people = GetPeople();

        foreach (var p in people.Where(x => x.Age % 5 == 0))
        {
            res.Add(p);
        }
        
        res.Sort((x, y) => x.Age < y.Age ? -1 : 1);
        
        people.Count();
        

        return res;
    }

    [Benchmark]
    public List<Person> Better()
    {
        var res = new List<Person>();
        var people = GetPeople().Where(x => x.Age % 5 == 0).ToList();

        foreach (var p in people)
        {
            res.Add(p);
        }
        
        res.Sort((x, y) => x.Age < y.Age ? -1 : 1);

        people.Count();

        return res;
    }
    public record Person(string Name, int Age);

    private IEnumerable<Person> GetPeople()
    {
        var lines = File.ReadAllLines("./people.csv");

        return lines.Select(x =>
        {
            var splitLine = x.Split(",");
            var name = splitLine.First();
            var age = splitLine.Last();
            return new Person(name, int.Parse(age));
        });

        // foreach (var line in lines)
        // {
        //     var splitLine = line.Split(",");
        //     var name = splitLine.First();
        //     var age = splitLine.Last();
        //     yield return new Person(name, int.Parse(age));
        // }
    }
}
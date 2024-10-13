using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchmarkingDataOperations;

[SimpleJob(RuntimeMoniker.Net80, baseline: true)]
[SimpleJob(RuntimeMoniker.Net90)]
[HideColumns("Job", "Median", "Error", "StdDev")]
public class BenchmarkLinqOperations
{
    private IEnumerable<int> _list = Enumerable.Range(0, 1000).ToList();

    [Benchmark] public bool Any() => _list.Any(i => i == 1000);
    [Benchmark] public bool All() => _list.Any(i => i >= 0);
    [Benchmark] public bool Count() => _list.Any(i => i == 0);
}
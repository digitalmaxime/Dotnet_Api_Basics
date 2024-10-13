using BenchmarkDotNet.Running;
using BenchmarkingDataOperations;


BenchmarkRunner.Run<BenchmarkQueryableVsEnumerable>();

// var b = new BenchmarkQueryableVsEnumerable();
// b.Dunno();
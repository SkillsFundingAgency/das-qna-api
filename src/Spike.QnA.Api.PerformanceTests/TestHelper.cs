using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spike.QnA.Api.PerformanceTests
{
    public class TestHelper
    {
        public const string SeedApplicationId = "C1824960-BA48-4175-817A-08D850B58946";
        public const string SeedWorkflowId = "5AEB59AC-4325-49CD-AD56-FF3B9E891D8C";
        public const string SeedSectionId = "2BC65FDE-D387-4D52-BE68-4D2EF532BC10";  //PreAmble
        public const string PreambleApplicationSectionId = "78B9828E-C789-4DA8-87C9-A4446562767F";

        public const string BaseApiUri = "https://localhost:44368";
        public string GetApplicationSectionsResource(string applicationId = SeedApplicationId) =>
            $"Applications/{applicationId}/sections";
        public string GetApplicationSectionResource(string applicationId = SeedApplicationId, string sectionId = SeedSectionId) =>
            $"Applications/{applicationId}/sections/{sectionId}";

        public string GetCreateApplicationResource() => $"Applications/start";

        public async Task WarmUp(int warmUpCount, Func<Task<HttpResponseMessage>> apiOperation)
        {
            var warmUpStats = new List<(int Index, TimeSpan Duration, bool Success)>();
            var warmupStopWatch = Stopwatch.StartNew();
            for (var i = 0; i < warmUpCount; i++)
            {
                var stopwatch = Stopwatch.StartNew();

                var response = await apiOperation();
                //                response.IsSuccessStatusCode.Should().BeTrue();
                stopwatch.Stop();
                warmUpStats.Add((i, stopwatch.Elapsed, response.IsSuccessStatusCode));
            }
            warmupStopWatch.Stop();
            Console.WriteLine($"Warm up stats. Total: {warmupStopWatch.ElapsedMilliseconds}ms.  Min: {warmUpStats.Min(x => x.Duration).TotalMilliseconds}ms, Average:{warmUpStats.Average(x => x.Duration.TotalMilliseconds)}ms, Max: {warmUpStats.Max(x => x.Duration).TotalMilliseconds}, Failures:{warmUpStats.Count(x => !x.Success)}");
        }
    }
}
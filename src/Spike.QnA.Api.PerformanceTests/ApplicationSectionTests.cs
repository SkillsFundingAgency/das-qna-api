using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Spike.QnA.Api.PerformanceTests
{
    [TestFixture]
    public class ApplicationSectionTests
    {
        private HttpClient httpClient;
        private TestHelper testHelper;
        
        [OneTimeSetUp]
        public void OneTimeSetUP()
        {
            httpClient = new HttpClient { BaseAddress = new Uri(TestHelper.BaseApiUri) };
            testHelper = new TestHelper();
        }

        [Test]
        public async Task Single_Call()
        {
            var response = await httpClient.GetAsync(testHelper.GetApplicationSectionsResource());
            response.IsSuccessStatusCode.Should().BeTrue();
        }


        [TestCase(5, 10)]
        public async Task Get_All_Application_Sections(int warmUpCount, int testDurationInSeconds)
        {
            await testHelper.WarmUp(warmUpCount, async () => await httpClient.GetAsync(testHelper.GetApplicationSectionsResource()));

            var testStats = new List<(TimeSpan Duration, bool Success)>();
            var testStopWatch = Stopwatch.StartNew();
            var stopTime = DateTime.Now.AddSeconds(testDurationInSeconds);
            while (DateTime.Now < stopTime)
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await httpClient.GetAsync(testHelper.GetApplicationSectionsResource());
                stopwatch.Stop();
                testStats.Add((stopwatch.Elapsed, response.IsSuccessStatusCode));
            }
            testStopWatch.Stop();
            testStats.Any(x => x.Success).Should().BeTrue("all tests failed.");
            Console.WriteLine($"Test stats. Total: {testStopWatch.ElapsedMilliseconds}ms. Calls: {testStats.Count},  Min: {testStats.Min(x => x.Duration).TotalMilliseconds}ms, Average:{testStats.Average(x => x.Duration.TotalMilliseconds)}ms, Max: {testStats.Max(x => x.Duration).TotalMilliseconds}, Failures:{testStats.Count(x => !x.Success)}");
        }

        [TestCase(5, 10)]
        public async Task Get_Application_Section(int warmUpCount, int testDurationInSeconds)
        {
            //await testHelper.WarmUp(warmUpCount, async () => await httpClient.GetAsync(testHelper.GetApplicationSectionResource(sectionId:TestHelper.PreambleApplicationSectionId)));
            await testHelper.WarmUp(warmUpCount, async () => await httpClient.GetAsync(testHelper.GetApplicationSectionResource()));

            var testStats = new List<(TimeSpan Duration, bool Success)>();
            var testStopWatch = Stopwatch.StartNew();
            var stopTime = DateTime.Now.AddSeconds(testDurationInSeconds);
            while (DateTime.Now < stopTime)
            {
                var stopwatch = Stopwatch.StartNew();
//                var response = await httpClient.GetAsync(testHelper.GetApplicationSectionResource(sectionId: TestHelper.PreambleApplicationSectionId));
                var response = await httpClient.GetAsync(testHelper.GetApplicationSectionResource());
                stopwatch.Stop();
                testStats.Add((stopwatch.Elapsed, response.IsSuccessStatusCode));
            }
            testStopWatch.Stop();

            testStats.Any(x => x.Success).Should().BeTrue("all tests failed.");
            Console.WriteLine($"Test stats. Total: {testStopWatch.ElapsedMilliseconds}ms. Calls: {testStats.Count}, Min: {testStats.Min(x => x.Duration).TotalMilliseconds}ms, Average:{testStats.Average(x => x.Duration.TotalMilliseconds)}ms, Max: {testStats.Max(x => x.Duration).TotalMilliseconds}, Failures:{testStats.Count(x => !x.Success)}");
        }
    }
}
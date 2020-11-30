using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Spike.QnA.Api.PerformanceTests
{
    public class StartApplicationRequest
    {
        public string UserReference { get; set; }
        public string WorkflowType { get; set; }
        public string ApplicationData { get; set; }
    }

    public class ApplicationData
    {
        public string OrganisationReferenceId { get; set; }
        public string OrganisationName { get; set; }
        public string ApplyProviderRoute { get; set; }
    }

    [TestFixture]
    public class ApplicationTests
    {
        private HttpClient httpClient;
        private TestHelper testHelper;

        [OneTimeSetUp]
        public void OneTimeSetUP()
        {
            httpClient = new HttpClient { BaseAddress = new Uri(TestHelper.BaseApiUri) };
            testHelper = new TestHelper();
        }

        [SetUp]
        public void SetUp()
        {
        }

        private StartApplicationRequest CreateStartApplicationRequest()
        {
            var providerRef = new Random(Guid.NewGuid().GetHashCode()).Next(99999999).ToString("0000000000");
            var applicationData = new ApplicationData
            {
                ApplyProviderRoute = "1",
                OrganisationName = $"Test Provider - {providerRef}",
                OrganisationReferenceId = providerRef
            };
            var startApplicationRequest = new StartApplicationRequest
            {
                WorkflowType = "roatp",
                UserReference = Guid.NewGuid().ToString("D"),
                ApplicationData = JsonConvert.SerializeObject(applicationData)
            };
            return startApplicationRequest;
        }

        [Test]
        public async Task Single_Call()
        {
            var json = JsonConvert.SerializeObject(CreateStartApplicationRequest());
            Console.WriteLine($"Request json: {json}");
            var response = await httpClient.PostAsync(testHelper.GetCreateApplicationResource(),
                new StringContent(json,Encoding.UTF8,"application/json"));
            response.IsSuccessStatusCode.Should().BeTrue(response.Content.ToString());
        }

        [TestCase(5, 10)]
        public async Task Create_Application(int warmUpCount, int testDurationInSeconds)
        {
            await testHelper.WarmUp(warmUpCount, async () =>
            {
                var json = JsonConvert.SerializeObject(CreateStartApplicationRequest());
                return await httpClient.PostAsync(testHelper.GetCreateApplicationResource(),
                    new StringContent(json, Encoding.UTF8, "application/json"));
            });

            var testStats = new List<(TimeSpan Duration, bool Success)>();
            var testStopWatch = Stopwatch.StartNew();
            var stopTime = DateTime.Now.AddSeconds(testDurationInSeconds);
            while (DateTime.Now < stopTime)
            {
                var json = JsonConvert.SerializeObject(CreateStartApplicationRequest());
                var stopwatch = Stopwatch.StartNew();
                var response = await httpClient.PostAsync(testHelper.GetCreateApplicationResource(),
                    new StringContent(json, Encoding.UTF8, "application/json"));
                stopwatch.Stop();
                testStats.Add((stopwatch.Elapsed, response.IsSuccessStatusCode));
            }
            testStopWatch.Stop();
            testStats.Any(x => x.Success).Should().BeTrue("all tests failed.");
            Console.WriteLine($"Test stats. Total: {testStopWatch.ElapsedMilliseconds}ms. Calls: {testStats.Count}, Min: {testStats.Min(x => x.Duration).TotalMilliseconds}ms, Average:{testStats.Average(x => x.Duration.TotalMilliseconds)}ms, Max: {testStats.Max(x => x.Duration).TotalMilliseconds}, Failures:{testStats.Count(x => !x.Success)}");
        }
    }
}
using System;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace OhmGraphite.Test
{
    public class GraphiteTest
    {
        [Fact, Trait("Category", "integration")]
        public async void InsertGraphiteTest()
        {
            var writer = new GraphiteWriter("graphite", 2003, "my-pc", tags: false);
            await writer.ReportMetrics(DateTime.Now, TestSensorCreator.Values());

            // wait for carbon to sync to disk
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var client = new HttpClient();
            var resp = await client.GetAsync("http://graphite/render?format=csv&target=ohm.my-pc.intelcpu.0.temperature.cpucore.1");
            Assert.True(resp.IsSuccessStatusCode);
            var content = await resp.Content.ReadAsStringAsync();
            Assert.Contains("ohm.my-pc.intelcpu.0.temperature.cpucore.1", content);
        }

        [Fact, Trait("Category", "integration")]
        public async void InsertTagGraphiteTest()
        {
            var writer = new GraphiteWriter("graphite", 2003, "my-pc", tags: true);
            await writer.ReportMetrics(DateTime.Now, TestSensorCreator.Values());

            // wait for carbon to sync to disk
            Thread.Sleep(TimeSpan.FromSeconds(2));
            var client = new HttpClient();
            var resp = await client.GetAsync("http://graphite/render?format=csv&target=seriesByTag('sensor_type=Temperature','hardware_type=CPU')");
            Assert.True(resp.IsSuccessStatusCode);
            var content = await resp.Content.ReadAsStringAsync();
            Assert.Contains("host=my-pc", content);
            Assert.Contains("app=ohm", content);
            Assert.Contains("sensor_type=Temperature", content);
        }
    }
}

using System;
using Xunit;
using AggregateGDPPopulation;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AggregateGDPPopulation.Tests
{
    public class AggregateDataTest
    {
        [Fact]
        public async void ShouldBeSameAsExpectedOutput()
        {
            Aggregate classagg = new Aggregate();
            await classagg.AggregateGDP2012Population(@"../../../../AggregateGDPPopulation/data/datafile.csv");
            string actualFile = File.ReadAllText("../../../expected-output.json");
            string expectedFile = File.ReadAllText(Environment.CurrentDirectory + @"/output/output.json");
            Dictionary<string, GDPPopulation> actual = JsonConvert.DeserializeObject<Dictionary<string, GDPPopulation>>(actualFile);
            Dictionary<string, GDPPopulation> expected = JsonConvert.DeserializeObject<Dictionary<string, GDPPopulation>>(expectedFile);
            foreach (var key in actual.Keys)
            {
                if (expected.ContainsKey(key))
                {
                    Assert.Equal(actual[key].GDP_2012, expected[key].GDP_2012);
                    Assert.Equal((double)actual[key].POPULATION_2012, (double)expected[key].POPULATION_2012);
                }
                else
                {
                    Assert.True(false);
                }
            }
        }
    }
}

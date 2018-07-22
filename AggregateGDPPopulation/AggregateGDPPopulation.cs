using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AggregateGDPPopulation
{
    public class Aggregate
    {
        string outputFileName = "/output.json";
        string outputFilePath = Environment.CurrentDirectory + @"/output";
        string countryJsonPath = @"../../../../AggregateGDPPopulation/data/country.json";
        public async Task<string> ReadAsync(string filePath)
        {
            string result;
            using (StreamReader reader = new StreamReader(filePath))
            {
                result = await reader.ReadToEndAsync();
            }
            return result;
        }

        public async Task WriteFile(Dictionary<String, GDPPopulation> dictionaryOfObjects)
        {
            if (!Directory.Exists(outputFilePath))
            {
                Directory.CreateDirectory(outputFilePath);
            }
            string JsonObject = JsonConvert.SerializeObject(dictionaryOfObjects);
            using (StreamWriter writer = new StreamWriter(outputFilePath + outputFileName))
            {
                await writer.WriteAsync(JsonObject);
            }
        }
        public async Task AggregateGDP2012Population(string filePath)
        {
            var csvfile = ReadAsync(filePath);
            var jsonfile = ReadAsync(countryJsonPath);
            await Task.WhenAll(csvfile, jsonfile);
            var Mapper = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonfile.Result);
            string[] CsvProcessed = csvfile.Result.Replace("\"", String.Empty).Trim().Split('\n');
            string[] headers = CsvProcessed[0].Split(',');
            int populationIndex = Array.IndexOf(headers, "Population (Millions) 2012");
            int gdpIndex = Array.IndexOf(headers, "GDP Billions (USD) 2012");
            int countriesIndex = Array.IndexOf(headers, "Country Name");
            Dictionary<string, GDPPopulation> dictonaryObject = new Dictionary<string, GDPPopulation>();
            for (int i = 1; i < csvfile.Result.Length; i++)
            {
                try
                {
                    string[] row = CsvProcessed[i].Split(',');
                    string countryName = row[countriesIndex];
                    string nameOfContinent = Mapper[countryName];
                    float Population = float.Parse(row[populationIndex]);
                    float Gdp = float.Parse(row[gdpIndex]);
                    try
                    {
                        dictonaryObject[nameOfContinent].GDP_2012 += Gdp;
                        dictonaryObject[nameOfContinent].POPULATION_2012 += Population;
                    }
                    catch(Exception e) {

                    }
                    finally
                    {
                        GDPPopulation Object = new GDPPopulation() { GDP_2012 = Gdp, POPULATION_2012 = Population };
                        dictonaryObject.Add(nameOfContinent, Object);
                    }
                }
                catch(Exception e) {

                }
            }
            await WriteFile(dictonaryObject);
        }
    }

    public class GDPPopulation
    {
        public float GDP_2012 { get; set; }
        public float POPULATION_2012 { get; set; }
    }

}



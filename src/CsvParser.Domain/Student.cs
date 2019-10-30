using CsvParser.Domain.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsvParser.Domain
{
    public class Student : IOneToManyEntity<Lesson, int>
    {
        public string Name { get; set; }

        [JsonIgnore]
        public IDictionary<Lesson, int> Associations { get; set; } = new Dictionary<Lesson, int>();

        public double AverageRate => Math.Round(Associations.Values.Sum() / (double)Associations.Values.Count(), 2);
    }
}

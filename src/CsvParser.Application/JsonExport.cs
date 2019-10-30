using CsvParser.Domain;
using CsvParser.Domain.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvParser.Application
{
    public class JsonExport : IExportService
    {
        public const string FILE_FORMAT = "json";
        public string FileFormat => FILE_FORMAT;

        public void Export(string filePath, IEnumerable<Student> students, IEnumerable<Lesson> lessons)
        {
            var averageGroup = students.Sum(s => s.AverageRate) / (double)students.Count();
            var averageLessons = lessons.Sum(s => s.Average) / (double)lessons.Count();

            var aggregation = new
            {
                Group = new
                {
                    students,
                    averageGroup,
                },
                Lessons = new
                {
                    lessons,
                    averageLessons
                }
            };

            var studentsJson = JsonConvert.SerializeObject(aggregation);

            using (var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, $"{filePath}.{FileFormat}"), FileMode.CreateNew))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(studentsJson);
            }
        }
    }
}

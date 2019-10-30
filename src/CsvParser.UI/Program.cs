using CsvParser.Application;
using CsvParser.Domain;
using CsvParser.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvParser.UI
{
    class Program
    {
        private static IEnumerable<Student> students;
        private static IEnumerable<Lesson> lessons;

        private const int INPUT_FILE_PATH_ARGS_INDEX = 0;
        private const int OUTPUT_FILE_PATH_ARGS_INDEX = 1;
        private const int TARGET_FILE_FORMAT_ARGS_INDEX = 2;

        private static string inputFilePath;
        private static string outputFilePath;
        private static string targerFormat;

        private static string GetFilePath(string fileName) => 
            Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName, "Data", fileName);

        private static IDictionary<string, IExportService> _services = 
            new Dictionary<string, IExportService>(StringComparer.OrdinalIgnoreCase)
        {
            { JsonExport.FILE_FORMAT, new JsonExport() },
            { ExcelExport.FILE_FORMAT, new ExcelExport() },
        };

        static void Main(string[] args)
        {
            try
            {
                
                if (args.Length < 3)
                {
                    throw new InvalidOperationException("You should specify 3 arguments.");
                }

                inputFilePath = GetFilePath(args[INPUT_FILE_PATH_ARGS_INDEX]);
                outputFilePath = GetFilePath(args[OUTPUT_FILE_PATH_ARGS_INDEX]);
                targerFormat = args[TARGET_FILE_FORMAT_ARGS_INDEX];

                if (!_services.TryGetValue(targerFormat, out var service))
                {
                    throw new InvalidOperationException($"Unsupported export file type '{targerFormat}'.");
                }

                ParseCsvFile();

                service.Export(outputFilePath, students, lessons);

                Console.WriteLine($"File '{outputFilePath}.{service.FileFormat}' successfylly created.");    
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void ParseCsvFile()
        {
            var csvParser = new Application.CsvParser();
            lessons = csvParser.ReadRowToEntities<Lesson>(inputFilePath, 1);
            students = csvParser.ReadColumnToEntities<Student>(inputFilePath, 1);
            csvParser.MapOneToMany<Student, Lesson, int>(inputFilePath, students, lessons);
            InitLessonsAverage(students, lessons);
        }

        private static void InitLessonsAverage(IEnumerable<Student> students, IEnumerable<Lesson> lessons)
        {
            var allRates = students.SelectMany(s => s.Associations);

            foreach (var lesson in lessons)
            {
                var lessonRates = allRates.Where(lr => lr.Key.Equals(lesson));
                lesson.Average = lessonRates.Sum(lr => lr.Value) / (double)lessonRates.Count();
            }
        }
    }
}

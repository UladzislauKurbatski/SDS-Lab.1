using CsvParser.Domain;
using CsvParser.Domain.Abstractions;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvParser.Application
{
    public class ExcelExport : IExportService
    {
        public const string FILE_FORMAT = "xlsx";
        public string FileFormat => FILE_FORMAT;

        public void Export(string filePath, IEnumerable<Student> students, IEnumerable<Lesson> lessons)
        {
            var fileStream = new FileStream($"{filePath}.{FileFormat}", FileMode.CreateNew);
            using (ExcelPackage excelPackage = new ExcelPackage(fileStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Students");
                worksheet.Cells[1, 1].Value = "Students";
                worksheet.Cells[1, 2].Value = "Average Rate";

                var row = 2;
                foreach (var student in students)
                {
                    worksheet.Cells[row, 1].Value = student.Name;
                    worksheet.Cells[row++, 2].Value = student.AverageRate;
                }

                var averageGroup = students.Sum(s => s.AverageRate) / (double)students.Count();
                worksheet.Cells[row++, 2].Value = averageGroup;

                worksheet = excelPackage.Workbook.Worksheets.Add("Lessons");
                worksheet.Cells[1, 1].Value = "Lessons";
                worksheet.Cells[1, 2].Value = "Average Rate";

                row = 2;
                foreach (var lesson in lessons)
                {
                    worksheet.Cells[row, 1].Value = lesson.Name;
                    worksheet.Cells[row++, 2].Value = lesson.Average;
                }

                averageGroup = lessons.Sum(s => s.Average) / (double)lessons.Count();
                worksheet.Cells[row++, 2].Value = averageGroup;

                excelPackage.Save();
            }
        }
    }
}

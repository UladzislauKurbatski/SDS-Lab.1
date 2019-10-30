using System.Collections.Generic;

namespace CsvParser.Domain.Abstractions
{
    public interface IExportService
    {
        string FileFormat { get; }
        void Export(string filePath, IEnumerable<Student> students, IEnumerable<Lesson> lessons);
    }
}

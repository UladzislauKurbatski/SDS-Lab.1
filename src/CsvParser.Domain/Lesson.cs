using CsvParser.Domain.Abstractions;
using System;

namespace CsvParser.Domain
{
    public class Lesson : INamedEntity
    {
        public string Name { get; set; }

        public double Average { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Lesson lesson))
                return false;

            return string.Equals(Name, lesson.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}

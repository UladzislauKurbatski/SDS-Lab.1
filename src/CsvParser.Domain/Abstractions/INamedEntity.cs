using System.Collections.Generic;

namespace CsvParser.Domain.Abstractions
{
    public interface INamedEntity
    {
        string Name { get; set; }
    }

    public interface IOneToManyEntity<TKey, TValue> 
        : INamedEntity 
        where TKey : INamedEntity
    {
        IDictionary<TKey, TValue> Associations { get; set; }
    }
}

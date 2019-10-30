using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using CsvParser.Domain.Abstractions;

namespace CsvParser.Application
{
    public class CsvParser
    {
        public IEnumerable<T> ReadRowToEntities<T>(string filePath, int offset = default(int)) 
            where T: INamedEntity, new()
        {
            using (var reader = new StreamReader(filePath, encoding: Encoding.UTF8))
            using (var csv = new CsvReader(reader))
            {
                var entities = new List<T>();

                csv.Read();

                while (csv.TryGetField<string>(offset++, out var field))
                {
                    entities.Add(new T() { Name = field });
                }

                return entities;
            }
        }

        public IEnumerable<T> ReadColumnToEntities<T>(string filePath, int offset = default(int))
            where T : INamedEntity, new()
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader))
            {
                var entities = new List<T>();

                while (offset-- != 0)
                    if (!csv.Read()) return entities;
                
                while (csv.Read())
                {
                    entities.Add(new T() { Name = csv.GetField(0) });
                }

                return entities;
            }
        }
        
        public void MapOneToMany<TOneEntity, TManyEntity, TValue>(string filePath, IEnumerable<TOneEntity> oneEntities, IEnumerable<TManyEntity> manyEntities)
            where TOneEntity : IOneToManyEntity<TManyEntity, TValue>
            where TManyEntity : INamedEntity
        {
            using (var reader = new StreamReader(filePath, encoding: Encoding.UTF8))
            using (var csv = new CsvReader(reader))
            {
                csv.Read();
                csv.ReadHeader();
                csv.Read();

                Dictionary<int, TManyEntity> idxAssociations = new Dictionary<int, TManyEntity>();
                
                foreach (var manyEntity in manyEntities)
                {
                    var headerIdx = csv.GetFieldIndex(manyEntity.Name, isTryGet: true);
                    if (headerIdx < 0) continue;
                    idxAssociations.Add(headerIdx, manyEntity);
                }
                
                while (csv.TryGetField<string>(0, out var oneEntityName))
                {
                    var oneEntity = oneEntities
                        .FirstOrDefault(e => string.Equals(e.Name, oneEntityName, System.StringComparison.OrdinalIgnoreCase));
                    if (oneEntity == null) continue;

                    var colIdx = 1;
                    while(csv.TryGetField<TValue>(colIdx, out var associationValue))
                    {
                        if (idxAssociations.TryGetValue(colIdx, out var manyEntity))
                        {
                            oneEntity.Associations.Add(manyEntity, associationValue);
                        }
                        colIdx++;
                    }
                    csv.Read();
                }

                return;
            }
        }
    }
}
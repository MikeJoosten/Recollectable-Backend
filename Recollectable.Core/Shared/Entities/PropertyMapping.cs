using Recollectable.Core.Shared.Interfaces;
using System.Collections.Generic;

namespace Recollectable.Core.Shared.Entities
{
    public class PropertyMapping<T> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary;
        }
    }
}
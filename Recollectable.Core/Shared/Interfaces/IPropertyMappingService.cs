using Recollectable.Core.Shared.Entities;
using System.Collections.Generic;

namespace Recollectable.Core.Shared.Interfaces
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}
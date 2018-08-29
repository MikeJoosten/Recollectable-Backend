using Recollectable.Core.Entities.Common;
using System.Collections.Generic;

namespace Recollectable.Core.Interfaces.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}
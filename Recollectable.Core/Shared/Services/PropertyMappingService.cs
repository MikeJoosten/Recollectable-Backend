using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.Core.Shared.Services
{
    public class PropertyMappingService
    {
        public static Dictionary<string, PropertyMappingValue> UserPropertyMapping =>
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) },
                { "Email", new PropertyMappingValue(new List<string>() { "Email" }) }
        };

        public static Dictionary<string, PropertyMappingValue> CurrencyPropertyMapping =>
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "Value", new PropertyMappingValue(new List<string>() { "Country.Name", "Type", "FaceValue", "ReleaseDate" }) },
                { "Country", new PropertyMappingValue(new List<string>() { "Country.Name" }) },
                { "ReleaseDate", new PropertyMappingValue(new List<string>() { "ReleaseDate" }) }
            };

        public static Dictionary<string, PropertyMappingValue> CollectionCollectablePropertyMapping =>
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "Country", new PropertyMappingValue(new List<string>() { "Collectable.Country.Name" }) },
                { "ReleaseDate", new PropertyMappingValue(new List<string>() { "Collectable.ReleaseDate" }) }
            };

        public static Dictionary<string, PropertyMappingValue> CollectionPropertyMapping =>
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "Type", new PropertyMappingValue(new List<string>() { "Type" }) }
            };

        public static Dictionary<string, PropertyMappingValue> CountryPropertyMapping =>
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) }
            };

        public static Dictionary<string, PropertyMappingValue> CollectorValuePropertyMapping =>
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) }
            };

        private static IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>
        {
            new PropertyMapping<User>(UserPropertyMapping),
            new PropertyMapping<Collection>(CollectionPropertyMapping),
            new PropertyMapping<Coin>(CurrencyPropertyMapping),
            new PropertyMapping<Banknote>(CurrencyPropertyMapping),
            new PropertyMapping<CollectionCollectable>(CollectionCollectablePropertyMapping),
            new PropertyMapping<Country>(CountryPropertyMapping),
            new PropertyMapping<CollectorValue>(CollectorValuePropertyMapping)
        };

        private static Dictionary<string, PropertyMappingValue> GetPropertyMapping<T>()
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<T>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(T)}>");
        }

        public static bool ValidMappingExistsFor<T>(string fields)
        {
            var propertyMapping = GetPropertyMapping<T>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedField :
                    trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
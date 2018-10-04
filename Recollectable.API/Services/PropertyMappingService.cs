using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Models.Collections;
using Recollectable.Core.Models.Locations;
using Recollectable.Core.Models.Users;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        public static readonly Dictionary<string, PropertyMappingValue> _userPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id.ToString()" }) },
                { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) },
                { "Email", new PropertyMappingValue(new List<string>() { "Email" }) }
            };

        public static readonly Dictionary<string, PropertyMappingValue> _currencyPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id.ToString()" }) },
                { "Value", new PropertyMappingValue(new List<string>() { "Country.Name", "Type", "FaceValue", "ReleaseDate" }) },
                { "Country", new PropertyMappingValue(new List<string>() { "Country.Name" }) },
                { "ReleaseDate", new PropertyMappingValue(new List<string>() { "ReleaseDate" }) }
            };

        public static readonly Dictionary<string, PropertyMappingValue> _collectablePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id.ToString()" }) },
                { "Country", new PropertyMappingValue(new List<string>() { "Collectable.Country.Name" }) },
                { "ReleaseDate", new PropertyMappingValue(new List<string>() { "Collectable.ReleaseDate" }) }
            };

        public static readonly Dictionary<string, PropertyMappingValue> _collectionPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id.ToString()" }) },
                { "Type", new PropertyMappingValue(new List<string>() { "Type" }) }
            };

        public static readonly Dictionary<string, PropertyMappingValue> _countryPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id.ToString()" }) },
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) }
            };

        public static readonly Dictionary<string, PropertyMappingValue> _collectorValuePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id.ToString()" }) }
            };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<UserDto, User>(_userPropertyMapping));
            propertyMappings.Add(new PropertyMapping<CollectionDto, Collection>(_collectionPropertyMapping));
            propertyMappings.Add(new PropertyMapping<CoinDto, Coin>(_currencyPropertyMapping));
            propertyMappings.Add(new PropertyMapping<BanknoteDto, Banknote>(_currencyPropertyMapping));
            propertyMappings.Add(new PropertyMapping<CollectableDto, Collectable>(_collectablePropertyMapping));
            propertyMappings.Add(new PropertyMapping<CountryDto, Country>(_countryPropertyMapping));
            propertyMappings.Add(new PropertyMapping<CollectorValueDto, CollectorValue>(_collectorValuePropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)}, {typeof(TDestination)}>");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

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
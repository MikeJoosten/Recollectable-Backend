using Microsoft.EntityFrameworkCore;
using Recollectable.Data.Helpers;
using Recollectable.Data.Services;
using Recollectable.Domain.Entities;
using Recollectable.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.Data.Repositories
{
    public class CoinRepository : ICoinRepository
    {
        private RecollectableContext _context;
        private ICountryRepository _countryRepository;
        private ICollectionRepository _collectionRepository;
        private IConditionRepository _conditionRepository;
        private IPropertyMappingService _propertyMappingService;

        public CoinRepository(RecollectableContext context, 
            ICountryRepository countryRepository, 
            ICollectionRepository collectionRepository,
            IConditionRepository conditionRepository,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _countryRepository = countryRepository;
            _collectionRepository = collectionRepository;
            _conditionRepository = conditionRepository;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Coin> GetCoins(CurrenciesResourceParameters resourceParameters)
        {
            var coins = _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<CoinDto, Coin>());

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                var type = resourceParameters.Type.Trim().ToLowerInvariant();
                coins = coins.Where(c => c.Type.ToLowerInvariant() == type);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                var country = resourceParameters.Country.Trim().ToLowerInvariant();
                coins = coins.Where(c => c.Country.Name.ToLowerInvariant() == country);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                coins = coins.Where(c => c.Country.Name.ToLowerInvariant().Contains(search)
                    || c.Type.ToLowerInvariant().Contains(search)
                    || c.ReleaseDate.ToLowerInvariant().Contains(search)
                    || c.Metal.ToLowerInvariant().Contains(search));
            }

            return PagedList<Coin>.Create(coins,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public IEnumerable<Coin> GetCoinsByCountry(Guid countryId)
        {
            if (_countryRepository.GetCountry(countryId) == null)
            {
                return null;
            }

            return _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => (c.FaceValue + " " + c.Type))
                .ThenBy(c => c.ReleaseDate);
        }

        public Coin GetCoin(Guid coinId)
        {
            return _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .FirstOrDefault(c => c.Id == coinId);
        }

        public void AddCoin(Coin coin)
        {
            if (coin.Id == Guid.Empty)
            {
                coin.Id = Guid.NewGuid();
            }

            if (coin.CountryId == Guid.Empty)
            {
                coin.CountryId = Guid.NewGuid();
            }

            if (coin.CollectorValueId == Guid.Empty)
            {
                coin.CollectorValueId = Guid.NewGuid();
            }

            _context.Coins.Add(coin);
        }

        public void UpdateCoin(Coin coin) { }

        public void DeleteCoin(Coin coin)
        {
            _context.Coins.Remove(coin);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool CoinExists(Guid coinId)
        {
            return _context.Coins.Any(c => c.Id == coinId);
        }
    }
}
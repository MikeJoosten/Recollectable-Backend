using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CoinRepository : IRepository<Coin, CurrenciesResourceParameters>
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CoinRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Coin> Get(CurrenciesResourceParameters resourceParameters)
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

        public Coin GetById(Guid coinId)
        {
            return _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .FirstOrDefault(c => c.Id == coinId);
        }

        public void Add(Coin coin)
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

        public void Update(Coin coin) { }

        public void Delete(Coin coin)
        {
            _context.Coins.Remove(coin);
        }

        public bool Exists(Guid coinId)
        {
            return _context.Coins.Any(c => c.Id == coinId);
        }
    }
}
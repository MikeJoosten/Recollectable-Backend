using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CoinRepository : ICoinRepository
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CoinRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<PagedList<Coin>> GetCoins(CurrenciesResourceParameters resourceParameters)
        {
            var coins = await _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<CoinDto, Coin>())
                .ToListAsync();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                var type = resourceParameters.Type.Trim().ToLowerInvariant();
                coins = coins.Where(c => c.Type.ToLowerInvariant() == type).ToList();
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                var country = resourceParameters.Country.Trim().ToLowerInvariant();
                coins = coins.Where(c => c.Country.Name.ToLowerInvariant() == country).ToList();
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                coins = coins.Where(c => c.Country.Name.ToLowerInvariant().Contains(search)
                    || c.Type.ToLowerInvariant().Contains(search)
                    || c.ReleaseDate.ToLowerInvariant().Contains(search)
                    || c.Metal.ToLowerInvariant().Contains(search)).ToList();
            }

            return PagedList<Coin>.Create(coins,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public async Task<Coin> GetCoinById(Guid coinId)
        {
            return await _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .FirstOrDefaultAsync(c => c.Id == coinId);
        }

        public void AddCoin(Coin coin)
        {
            if (coin.Id == Guid.Empty)
            {
                coin.Id = Guid.NewGuid();
            }

            _context.Coins.Add(coin);
        }

        public void UpdateCoin(Coin coin) { }

        public void DeleteCoin(Coin coin)
        {
            _context.Coins.Remove(coin);
        }

        public async Task<bool> Exists(Guid coinId)
        {
            return await _context.Coins.AnyAsync(c => c.Id == coinId);
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
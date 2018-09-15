using Microsoft.EntityFrameworkCore;
using Recollectable.Core.DTOs.Collectables;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Extensions;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CoinRepository : BaseRepository<Coin, CurrenciesResourceParameters>
    {
        private RecollectableContext _context;

        public CoinRepository(RecollectableContext context)
        {
            _context = context;
        }

        public override PagedList<Coin> Get(CurrenciesResourceParameters resourceParameters)
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

        public override Coin GetById(Guid coinId)
        {
            return _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .FirstOrDefault(c => c.Id == coinId);
        }

        public override void Add(Coin coin)
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

        public override void Update(Coin coin) { }

        public override void Delete(Coin coin)
        {
            _context.Coins.Remove(coin);
        }

        public override bool Exists(Guid coinId)
        {
            return _context.Coins.Any(c => c.Id == coinId);
        }
    }
}
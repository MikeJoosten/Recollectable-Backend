using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Collectables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class CoinService : ICoinService
    {
        private readonly IRepository<Coin> _coinRepository;

        public CoinService(IRepository<Coin> coinRepository)
        {
            _coinRepository = coinRepository;
        }

        public async Task<PagedList<Coin>> FindCoins(CurrenciesResourceParameters resourceParameters)
        {
            var coins = await _coinRepository.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                coins = await _coinRepository.GetAll(new CoinByType(resourceParameters.Type));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                coins = await _coinRepository.GetAll(new CoinByCountry(resourceParameters.Country));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                coins = await _coinRepository.GetAll(new CoinBySearch(resourceParameters.Search));
            }

            coins = coins.OrderBy(resourceParameters.OrderBy, PropertyMappingService.CurrencyPropertyMapping);

            return PagedList<Coin>.Create(coins.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Coin> FindCoinById(Guid id)
        {
            return await _coinRepository.GetSingle(new CoinById(id));
        }

        public async Task CreateCoin(Coin coin)
        {
            await _coinRepository.Add(coin);
        }

        public void UpdateCoin(Coin coin)
        {
            _coinRepository.Update(coin);
        }

        public void RemoveCoin(Coin coin)
        {
            _coinRepository.Delete(coin);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _coinRepository.Exists(new CoinById(id));
        }

        public async Task<bool> Save()
        {
            return await _coinRepository.Save();
        }
    }
}
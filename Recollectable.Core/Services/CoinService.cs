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
        private readonly IUnitOfWork _unitOfWork;

        public CoinService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<Coin>> FindCoins(CurrenciesResourceParameters resourceParameters)
        {
            var coins = await _unitOfWork.Coins.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Type) || !string.IsNullOrEmpty(resourceParameters.Country))
            {
                coins = await _unitOfWork.Coins.GetAll(new CoinByType(resourceParameters.Type) || new CoinByCountry(resourceParameters.Country));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                coins = await _unitOfWork.Coins
                    .GetAll(new CoinByType(resourceParameters.Type) || 
                    new CoinByCountry(resourceParameters.Country) || 
                    new CoinBySearch(resourceParameters.Search));
            }

            coins = coins.OrderBy(resourceParameters.OrderBy, PropertyMappingService.CurrencyPropertyMapping);

            return PagedList<Coin>.Create(coins.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Coin> FindCoinById(Guid id)
        {
            return await _unitOfWork.Coins.GetSingle(new CoinById(id));
        }

        public async Task CreateCoin(Coin coin)
        {
            await _unitOfWork.Coins.Add(coin);
        }

        public void UpdateCoin(Coin coin) { }

        public void RemoveCoin(Coin coin)
        {
            _unitOfWork.Coins.Delete(coin);
        }

        public async Task<bool> CoinExists(Guid id)
        {
            var coin = await _unitOfWork.Coins.GetSingle(new CoinById(id));
            return coin != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}
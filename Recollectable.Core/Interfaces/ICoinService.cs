using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface ICoinService
    {
        Task<PagedList<Coin>> FindCoins(CurrenciesResourceParameters resourceParameters);
        Task<Coin> FindCoinById(Guid id);
        Task CreateCoin(Coin coin);
        void UpdateCoin(Coin coin);
        void RemoveCoin(Coin coin);
        Task<bool> CoinExists(Guid id);
        Task<bool> Save();
    }
}
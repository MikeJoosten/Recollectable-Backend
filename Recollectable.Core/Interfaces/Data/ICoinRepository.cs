using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface ICoinRepository
    {
        Task<PagedList<Coin>> GetCoins(CurrenciesResourceParameters resourceParameters);
        Task<Coin> GetCoinById(Guid id);
        void AddCoin(Coin coin);
        void UpdateCoin(Coin coin);
        void DeleteCoin(Coin coin);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using System;

namespace Recollectable.Core.Interfaces.Repositories
{
    public interface ICoinRepository
    {
        PagedList<Coin> GetCoins(CurrenciesResourceParameters resourceParameters);
        Coin GetCoin(Guid coinId);
        void AddCoin(Coin coin);
        void UpdateCoin(Coin coin);
        void DeleteCoin(Coin coin);
        bool Save();
        bool CoinExists(Guid coinId);
    }
}
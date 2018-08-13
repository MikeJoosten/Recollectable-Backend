using Recollectable.Data.Helpers;
using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Recollectable.Data.Repositories
{
    public interface ICoinRepository
    {
        PagedList<Coin> GetCoins(CollectablesResourceParameters resourceParameters);
        IEnumerable<Coin> GetCoinsByCountry(Guid countryId);
        Coin GetCoin(Guid coinId);
        void AddCoin(Coin coin);
        void UpdateCoin(Coin coin);
        void DeleteCoin(Coin coin);
        bool Save();
        bool CoinExists(Guid coinId);
    }
}
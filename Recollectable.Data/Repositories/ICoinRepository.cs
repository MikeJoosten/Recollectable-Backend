using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface ICoinRepository
    {
        IEnumerable<Coin> GetCoins();
        IEnumerable<Coin> GetCoinsByCountry(Guid countryId);
        Coin GetCoin(Guid coinId);
        void AddCoin(Coin coin);
        void UpdateCoin(Coin coin);
        void DeleteCoin(Coin coin);
        bool Save();
        bool CoinExists(Guid coinId);
    }
}
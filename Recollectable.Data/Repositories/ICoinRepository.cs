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
        IEnumerable<Coin> GetCoinsByCollection(Guid collectionId);
        Coin GetCoin(Guid coinId);
        void AddCoin(Coin coin);
        void AddCoinToCollection(CollectionCollectable collectionCollectable);
        void UpdateCoin(Coin coin);
        void DeleteCoin(Coin coin);
        void DeleteCoinFromCollection(CollectionCollectable collectionCollectable);
        bool Save();
        bool CoinExists(Guid coinId);
    }
}
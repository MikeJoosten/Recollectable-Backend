using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface ICoinRepository
    {
        IEnumerable<Coin> GetCoins();
        IEnumerable<Coin> GetCoinsByCollection(Guid collectionId);
        Coin GetCoin(Guid coinId);
        Coin GetCoinByCollection(Guid collectionId, Guid coinId);
        void AddCoin(Coin coin);
        void UpdateCoin(Coin coin);
        void DeleteCoin(Coin coin);
    }
}
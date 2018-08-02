using Microsoft.EntityFrameworkCore;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public class CoinRepository : ICoinRepository
    {
        private RecollectableContext _context;
        private ICountryRepository _countryRepository;

        public CoinRepository(RecollectableContext context, 
            ICountryRepository countryRepository)
        {
            _context = context;
            _countryRepository = countryRepository;
        }

        public IEnumerable<Coin> GetCoins()
        {
            return _context.Coins.OrderBy(c => c.Country.Name);
        }

        public IEnumerable<Coin> GetCoinsByCountry(Guid countryId)
        {
            if (_countryRepository.GetCountry(countryId) == null)
            {
                return null;
            }

            return _context.Coins
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.Type);
        }

        public IEnumerable<Coin> GetCoinsByCollection(Guid collectionId)
        {
            return _context.Coins
                .Include(c => c.CollectionCollectables)
                .ThenInclude(cc => cc.CollectionId == collectionId)
                .OrderBy(c => c.Country.Name);
        }

        public Coin GetCoin(Guid coinId)
        {
            return _context.Coins.FirstOrDefault(c => c.Id == coinId);
        }

        public void AddCoin(Coin coin)
        {
            if (coin.Id == Guid.Empty)
            {
                coin.Id = Guid.NewGuid();
            }

            _context.Coins.Add(coin);
        }

        public void UpdateCoin(Coin coin) { }

        public void DeleteCoin(Coin coin)
        {
            _context.Coins.Remove(coin);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
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
        private ICollectionRepository _collectionRepository;
        private IConditionRepository _conditionRepository;

        public CoinRepository(RecollectableContext context, 
            ICountryRepository countryRepository, 
            ICollectionRepository collectionRepository,
            IConditionRepository conditionRepository)
        {
            _context = context;
            _countryRepository = countryRepository;
            _collectionRepository = collectionRepository;
            _conditionRepository = conditionRepository;
        }

        public IEnumerable<Coin> GetCoins()
        {
            return _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .OrderBy(c => c.Country.Name)
                .ThenBy(c => (c.FaceValue + " " + c.Type))
                .ThenBy(c => c.ReleaseDate);
        }

        public IEnumerable<Coin> GetCoinsByCountry(Guid countryId)
        {
            if (_countryRepository.GetCountry(countryId) == null)
            {
                return null;
            }

            return _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => (c.FaceValue + " " + c.Type))
                .ThenBy(c => c.ReleaseDate);
        }

        public Coin GetCoin(Guid coinId)
        {
            return _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .FirstOrDefault(c => c.Id == coinId);
        }

        public void AddCoin(Coin coin)
        {
            if (coin.Id == Guid.Empty)
            {
                coin.Id = Guid.NewGuid();
            }

            if (coin.CountryId == Guid.Empty)
            {
                coin.CountryId = Guid.NewGuid();
            }

            if (coin.CollectorValueId == Guid.Empty)
            {
                coin.CollectorValueId = Guid.NewGuid();
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

        public bool CoinExists(Guid coinId)
        {
            return _context.Coins.Any(c => c.Id == coinId);
        }
    }
}
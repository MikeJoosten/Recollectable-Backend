using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Infrastructure.Data.Repositories;
using System;

namespace Recollectable.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly RecollectableContext _context;

        public IRepository<Banknote, CurrenciesResourceParameters> BanknoteRepository =>
            new BanknoteRepository(_context);

        public IRepository<Coin, CurrenciesResourceParameters> CoinRepository =>
            new CoinRepository(_context);

        public IRepository<Collection, CollectionsResourceParameters> CollectionRepository =>
            new CollectionRepository(_context);

        public IRepository<CollectorValue, CollectorValuesResourceParameters> CollectorValueRepository =>
            new CollectorValueRepository(_context);

        public IRepository<Condition, ConditionsResourceParameters> ConditionRepository =>
            new ConditionRepository(_context);

        public IRepository<Country, CountriesResourceParameters> CountryRepository =>
            new CountryRepository(_context);

        public IRepository<User, UsersResourceParameters> UserRepository =>
            new UserRepository(_context);

        public ICollectableRepository CollectableRepository =>
            new CollectableRepository(_context, this);

        public UnitOfWork(RecollectableContext context)
        {
            _context = context;
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
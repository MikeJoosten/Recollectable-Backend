using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Infrastructure.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private RecollectableContext _context;

        public IRepository<Coin> Coins => new CoinRepository(_context);
        public IRepository<Banknote> Banknotes => new BanknoteRepository(_context);
        public IRepository<Collectable> Collectables => new CollectableRepository(_context);
        public IRepository<CollectionCollectable> CollectionCollectables => new CollectionCollectableRepository(_context);
        public IRepository<CollectorValue> CollectorValues => new CollectorValueRepository(_context);
        public IRepository<Condition> Conditions => new ConditionRepository(_context);
        public IRepository<Collection> Collections => new CollectionRepository(_context);
        public IRepository<Country> Countries => new CountryRepository(_context);
        public IRepository<User> Users => new UserRepository(_context);

        public UnitOfWork(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Infrastructure.Data.Repositories;
using System;

namespace Recollectable.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public IRepository<Banknote, CurrenciesResourceParameters> BanknoteRepository =>
            new BanknoteRepository(_context, _propertyMappingService);

        public IRepository<Coin, CurrenciesResourceParameters> CoinRepository =>
            new CoinRepository(_context, _propertyMappingService);

        public IRepository<Collection, CollectionsResourceParameters> CollectionRepository =>
            new CollectionRepository(_context, _propertyMappingService);

        public IRepository<CollectorValue, CollectorValuesResourceParameters> CollectorValueRepository =>
            new CollectorValueRepository(_context, _propertyMappingService);

        public IRepository<Country, CountriesResourceParameters> CountryRepository =>
            new CountryRepository(_context, _propertyMappingService);

        public IRepository<User, UsersResourceParameters> UserRepository =>
            new UserRepository(_context, _propertyMappingService);

        public ICollectableRepository CollectableRepository =>
            new CollectableRepository(_context, this, _propertyMappingService);

        public UnitOfWork(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
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
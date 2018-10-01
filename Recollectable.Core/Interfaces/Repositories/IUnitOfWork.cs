using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces.Services;

namespace Recollectable.Core.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        ICollectableRepository CollectableRepository { get; }
        IRepository<Banknote, CurrenciesResourceParameters> BanknoteRepository { get; }
        IRepository<Coin, CurrenciesResourceParameters> CoinRepository { get; }
        IRepository<Collection, CollectionsResourceParameters> CollectionRepository { get; }
        IRepository<CollectorValue, CollectorValuesResourceParameters> CollectorValueRepository { get; }
        IRepository<Country, CountriesResourceParameters> CountryRepository { get; }
        IRepository<User, UsersResourceParameters> UserRepository { get; }

        bool Save();
        void Dispose();
    }
}
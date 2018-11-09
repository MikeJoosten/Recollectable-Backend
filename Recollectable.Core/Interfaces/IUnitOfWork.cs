using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface IUnitOfWork
    {
        ICollectableRepository CollectableRepository { get; }
        ICollectorValueRepository CollectorValueRepository { get; }
        IRepository<Banknote, CurrenciesResourceParameters> BanknoteRepository { get; }
        IRepository<Coin, CurrenciesResourceParameters> CoinRepository { get; }
        IRepository<Collection, CollectionsResourceParameters> CollectionRepository { get; }
        IRepository<Country, CountriesResourceParameters> CountryRepository { get; }
        IRepository<User, UsersResourceParameters> UserRepository { get; }

        Task<bool> Save();
        void Dispose();
    }
}
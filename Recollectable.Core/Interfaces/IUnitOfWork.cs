using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.Users;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Coin> Coins { get; }
        IRepository<Banknote> Banknotes { get; }
        IRepository<Collectable> Collectables { get; }
        IRepository<CollectionCollectable> CollectionCollectables { get; }
        IRepository<CollectorValue> CollectorValues { get; }
        IRepository<Condition> Conditions { get; }
        IRepository<Collection> Collections { get; }
        IRepository<Country> Countries { get; }
        IRepository<User> Users { get; }

        Task<bool> Save();
        void Dispose();
    }
}
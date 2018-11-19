using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface ICollectionRepository
    {
        Task<PagedList<Collection>> GetCollections(CollectionsResourceParameters resourceParameters);
        Task<Collection> GetCollectionById(Guid id);
        void AddCollection(Collection collection);
        void UpdateCollection(Collection collection);
        void DeleteCollection(Collection collection);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}

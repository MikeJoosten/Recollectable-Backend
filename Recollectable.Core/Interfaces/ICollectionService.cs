using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface ICollectionService
    {
        Task<PagedList<Collection>> FindCollections(CollectionsResourceParameters resourceParameters);
        Task<Collection> FindCollectionById(Guid id);
        Task CreateCollection(Collection collection);
        void UpdateCollection(Collection collection);
        void RemoveCollection(Collection collection);
        Task<bool> CollectionExists(Guid id);
        Task<bool> Save();
    }
}
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using System;

namespace Recollectable.Core.Interfaces.Repositories
{
    public interface ICollectionRepository
    {
        PagedList<Collection> GetCollections
            (CollectionsResourceParameters resourceParameters);
        Collection GetCollection(Guid collectionId);
        void AddCollection(Collection collection);
        void UpdateCollection(Collection collection);
        void DeleteCollection(Collection collection);
        bool Save();
        bool CollectionExists(Guid collectionId);
    }
}
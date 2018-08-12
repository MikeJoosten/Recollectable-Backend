using Recollectable.Data.Helpers;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
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
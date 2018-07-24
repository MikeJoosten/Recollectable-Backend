using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface ICollectionRepository
    {
        IEnumerable<Collection> GetCollections();
        IEnumerable<Collection> GetCollectionsByUser(Guid userId);
        Collection GetCollection(Guid collectionId);
        Collection GetCollectionByUser(Guid userId, Guid collectionId);
        void AddCollection(Guid userId, Collection collection);
        void UpdateCollection(Collection collection);
        void DeleteCollection(Collection collection);
    }
}
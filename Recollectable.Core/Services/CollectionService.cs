using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly IRepository<Collection> _collectionRepository;

        public CollectionService(IRepository<Collection> collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }

        public async Task<PagedList<Collection>> FindCollections(CollectionsResourceParameters resourceParameters)
        {
            var collections = await _collectionRepository.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                collections = await _collectionRepository.GetAll(new CollectionByType(resourceParameters.Type));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                collections = await _collectionRepository.GetAll(new CollectionBySearch(resourceParameters.Search));
            }

            collections = collections.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CollectionPropertyMapping);

            return PagedList<Collection>.Create(collections.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Collection> FindCollectionById(Guid id)
        {
            return await _collectionRepository.GetSingle(new CollectionById(id));
        }

        public async Task CreateCollection(Collection collection)
        {
            await _collectionRepository.Add(collection);
        }

        public void UpdateCollection(Collection collection)
        {
            _collectionRepository.Update(collection);
        }

        public void RemoveCollection(Collection collection)
        {
            _collectionRepository.Delete(collection);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _collectionRepository.Exists(new CollectionById(id));
        }

        public async Task<bool> Save()
        {
            return await _collectionRepository.Save();
        }
    }
}
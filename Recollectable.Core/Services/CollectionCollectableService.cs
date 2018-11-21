using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Collectables;
using Recollectable.Core.Specifications.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class CollectionCollectableService : ICollectionCollectableService
    {
        private readonly IRepository<CollectionCollectable> _collectionCollectableRepository;
        private readonly IRepository<Collection> _collectionRepository;
        private readonly IRepository<Collectable> _collectableRepository;

        public CollectionCollectableService(IRepository<CollectionCollectable> collectionCollectableRepository,
            IRepository<Collection> collectionRepository, IRepository<Collectable> collectableRepository)
        {
            _collectionCollectableRepository = collectionCollectableRepository;
            _collectionRepository = collectionRepository;
            _collectableRepository = collectableRepository;
        }

        public async Task<PagedList<CollectionCollectable>> FindCollectionCollectables
            (Guid collectionId, CollectionCollectablesResourceParameters resourceParameters)
        {
            if (!await _collectionRepository.Exists(new CollectionById(collectionId)))
            {
                return null;
            }

            var collectables = await _collectionCollectableRepository.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                collectables = await _collectionCollectableRepository.GetAll
                    (new CollectableByCollectionId(collectionId) && new CollectableByType(resourceParameters.Type));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                collectables = await _collectionCollectableRepository.GetAll
                    (new CollectableByCollectionId(collectionId) && new CollectableByCountry(resourceParameters.Country));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                collectables = await _collectionCollectableRepository.GetAll
                    (new CollectableByCollectionId(collectionId) && new CollectableBySearch(resourceParameters.Search));
            }

            collectables = collectables.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CollectablePropertyMapping);

            return PagedList<CollectionCollectable>.Create(collectables.ToList(),
                resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<CollectionCollectable> FindCollectionCollectableById(Guid collectionId, Guid id)
        {
            return await _collectionCollectableRepository.GetSingle
                (new CollectableByCollectionId(collectionId) && new CollectableById(id));
        }

        public async Task<Collectable> FindCollectableById(Guid collectableId)
        {
            return await _collectableRepository.GetSingle(new CollectableItemById(collectableId));
        }

        public async Task CreateCollectionCollectable(CollectionCollectable collectable)
        {
            await _collectionCollectableRepository.Add(collectable);
        }

        public void UpdateCollectionCollectable(CollectionCollectable collectable)
        {
            _collectionCollectableRepository.Update(collectable);
        }

        public void RemoveCollectionCollectable(CollectionCollectable collectable)
        {
            _collectionCollectableRepository.Delete(collectable);
        }

        public async Task<bool> Exists(Guid collectionId, Guid id)
        {
            return await _collectionCollectableRepository
                .Exists(new CollectableByCollectionId(collectionId) && new CollectableById(id));
        }

        public async Task<bool> Save()
        {
            return await _collectionCollectableRepository.Save();
        }
    }
}
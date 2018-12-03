using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Collectables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class CollectionCollectableService : ICollectionCollectableService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CollectionCollectableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<CollectionCollectable>> FindCollectionCollectables
            (Guid collectionId, CollectionCollectablesResourceParameters resourceParameters)
        {
            var collectables = await _unitOfWork.CollectionCollectables.GetAll(new CollectableByCollectionId(collectionId));

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                collectables = await _unitOfWork.CollectionCollectables.GetAll
                    (new CollectableByCollectionId(collectionId) && new CollectableByType(resourceParameters.Type));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                collectables = await _unitOfWork.CollectionCollectables.GetAll
                    (new CollectableByCollectionId(collectionId) && new CollectableByCountry(resourceParameters.Country));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                collectables = await _unitOfWork.CollectionCollectables.GetAll
                    (new CollectableByCollectionId(collectionId) && new CollectableBySearch(resourceParameters.Search));
            }

            collectables = collectables.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CollectablePropertyMapping);

            return PagedList<CollectionCollectable>.Create(collectables.ToList(),
                resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<CollectionCollectable> FindCollectionCollectableById(Guid collectionId, Guid id)
        {
            return await _unitOfWork.CollectionCollectables.GetSingle
                (new CollectableByCollectionId(collectionId) && new CollectableById(id));
        }

        public async Task<Collectable> FindCollectableById(Guid collectableId)
        {
            return await _unitOfWork.Collectables.GetSingle(new CollectableItemById(collectableId));
        }

        public async Task CreateCollectionCollectable(CollectionCollectable collectable)
        {
            await _unitOfWork.CollectionCollectables.Add(collectable);
        }

        public void UpdateCollectionCollectable(CollectionCollectable collectable) { }

        public void RemoveCollectionCollectable(CollectionCollectable collectable)
        {
            _unitOfWork.CollectionCollectables.Delete(collectable);
        }

        public async Task<bool> Exists(Guid collectionId, Guid id)
        {
            var collectionCollectable = await _unitOfWork.CollectionCollectables
                .GetSingle(new CollectableByCollectionId(collectionId) && new CollectableById(id));

            return collectionCollectable != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}
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
        private readonly IUnitOfWork _unitOfWork;

        public CollectionCollectableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<CollectionCollectable>> FindCollectionCollectables
            (Guid collectionId, CollectionCollectablesResourceParameters resourceParameters)
        {
            var collection = await _unitOfWork.Collections.GetSingle(new CollectionById(collectionId));

            if (collection == null)
            {
                return null;
            }

            var collectables = await _unitOfWork.CollectionCollectables.GetAll(new CollectionCollectableByCollectionId(collectionId));

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                collectables = await _unitOfWork.CollectionCollectables
                    .GetAll(new CollectionCollectableByCollectionId(collectionId) && 
                    new CollectionCollectableByCountry(resourceParameters.Country));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                collectables = await _unitOfWork.CollectionCollectables
                    .GetAll(new CollectionCollectableByCollectionId(collectionId) && 
                    (new CollectionCollectableByCountry(resourceParameters.Country) || 
                    new CollectionCollectableBySearch(resourceParameters.Search)));
            }

            collectables = collectables.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CollectionCollectablePropertyMapping);

            return PagedList<CollectionCollectable>.Create(collectables.ToList(),
                resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<CollectionCollectable> FindCollectionCollectableById(Guid collectionId, Guid id)
        {
            return await _unitOfWork.CollectionCollectables.GetSingle
                (new CollectionCollectableByCollectionId(collectionId) && new CollectionCollectableById(id));
        }

        public async Task<Collectable> FindCollectableById(Guid collectableId)
        {
            return await _unitOfWork.Collectables.GetSingle(new CollectableById(collectableId));
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

        public async Task<bool> CollectionCollectableExists(Guid collectionId, Guid id)
        {
            var collectionCollectable = await _unitOfWork.CollectionCollectables
                .GetSingle(new CollectionCollectableByCollectionId(collectionId) && new CollectionCollectableById(id));

            return collectionCollectable != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}
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
        private readonly IUnitOfWork _unitOfWork;

        public CollectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<Collection>> FindCollections(CollectionsResourceParameters resourceParameters)
        {
            var collections = await _unitOfWork.Collections.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                collections = await _unitOfWork.Collections.GetAll(new CollectionByType(resourceParameters.Type));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                collections = await _unitOfWork.Collections
                    .GetAll(new CollectionByType(resourceParameters.Type) || new CollectionBySearch(resourceParameters.Search));
            }

            collections = collections.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CollectionPropertyMapping);

            return PagedList<Collection>.Create(collections.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Collection> FindCollectionById(Guid id)
        {
            return await _unitOfWork.Collections.GetSingle(new CollectionById(id));
        }

        public async Task CreateCollection(Collection collection)
        {
            await _unitOfWork.Collections.Add(collection);
        }

        public void UpdateCollection(Collection collection) { }

        public void RemoveCollection(Collection collection)
        {
            _unitOfWork.Collections.Delete(collection);
        }

        public async Task<bool> CollectionExists(Guid id)
        {
            var collection = await _unitOfWork.Collections.GetSingle(new CollectionById(id));
            return collection != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}
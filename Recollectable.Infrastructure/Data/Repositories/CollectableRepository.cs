using Microsoft.EntityFrameworkCore;
using Recollectable.Core.DTOs.Collectables;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Services.Common;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectableRepository : ICollectableRepository
    {
        private RecollectableContext _context;
        private IUnitOfWork _unitOfWork;
        private IPropertyMappingService _propertyMappingService;

        public CollectableRepository(RecollectableContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _propertyMappingService = new PropertyMappingService();
        }

        public PagedList<CollectionCollectable> Get(Guid collectionId,
            CollectablesResourceParameters resourceParameters)
        {
            if (!_unitOfWork.CollectionRepository.Exists(collectionId))
            {
                return null;
            }

            var collectables = _context.CollectionCollectables
                .Include(cc => cc.Condition)
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.Country)
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.CollectorValue)
                .Where(cc => cc.CollectionId == collectionId)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<CollectableDto, Collectable>());

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                var country = resourceParameters.Country.Trim().ToLowerInvariant();
                collectables = collectables.Where(c =>
                    c.Collectable.Country.Name.ToLowerInvariant() == country);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                collectables = collectables.Where(c => c.Collectable.Country.Name.ToLowerInvariant().Contains(search)
                    || c.Collectable.ReleaseDate.ToLowerInvariant().Contains(search));
            }

            return PagedList<CollectionCollectable>.Create(collectables,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public CollectionCollectable GetById(Guid collectionId, Guid Id)
        {
            return _context.CollectionCollectables
                .Include(cc => cc.Condition)
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.Country)
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.CollectorValue)
                .Where(cc => cc.CollectionId == collectionId)
                .FirstOrDefault(cc => cc.Id == Id);
        }

        public Collectable GetCollectableItem(Guid collectableId)
        {
            return _context.Collectables.FirstOrDefault(c => c.Id == collectableId);
        }

        public void Add(CollectionCollectable collectable)
        {
            if (collectable.Id == Guid.Empty)
            {
                collectable.Id = Guid.NewGuid();
            }

            _context.CollectionCollectables.Add(collectable);
        }

        public void Update(CollectionCollectable collectable) { }

        public void Delete(CollectionCollectable collectable)
        {
            _context.CollectionCollectables.Remove(collectable);
        }

        public bool Exists(Guid Id)
        {
            return _context.CollectionCollectables.Any(cc => cc.Id == Id);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectableRepository : ICollectableRepository
    {
        private RecollectableContext _context;
        private IUnitOfWork _unitOfWork;
        private IPropertyMappingService _propertyMappingService;

        public CollectableRepository(RecollectableContext context, 
            IUnitOfWork unitOfWork, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<PagedList<CollectionCollectable>> Get(Guid collectionId,
            CollectablesResourceParameters resourceParameters)
        {
            if (!await _unitOfWork.CollectionRepository.Exists(collectionId))
            {
                return null;
            }

            var collectables = await _context.CollectionCollectables
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.Country)
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.CollectorValue)
                .Where(cc => cc.CollectionId == collectionId)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<CollectableDto, CollectionCollectable>())
                .ToListAsync();

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                var country = resourceParameters.Country.Trim().ToLowerInvariant();
                collectables = collectables.Where(c =>
                    c.Collectable.Country.Name.ToLowerInvariant() == country).ToList();
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                collectables = collectables.Where(c => c.Collectable.Country.Name.ToLowerInvariant().Contains(search)
                    || c.Collectable.ReleaseDate.ToLowerInvariant().Contains(search)).ToList();
            }

            return PagedList<CollectionCollectable>.Create(collectables,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public async Task<CollectionCollectable> GetById(Guid collectionId, Guid Id)
        {
            return await _context.CollectionCollectables
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.Country)
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.CollectorValue)
                .Where(cc => cc.CollectionId == collectionId)
                .FirstOrDefaultAsync(cc => cc.Id == Id);
        }

        public async Task<Collectable> GetCollectableItem(Guid collectableId)
        {
            return await _context.Collectables.FirstOrDefaultAsync(c => c.Id == collectableId);
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

        public async Task<bool> Exists(Guid collectionId, Guid Id)
        {
            return await _context.CollectionCollectables
                .Where(cc => cc.CollectionId == collectionId)
                .AnyAsync(cc => cc.Id == Id);
        }
    }
}
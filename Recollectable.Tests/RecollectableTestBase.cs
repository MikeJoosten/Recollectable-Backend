using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using Recollectable.Core.Services;
using Recollectable.Infrastructure.Data;
using System;

namespace Recollectable.Tests
{
    public class RecollectableTestBase
    {
        protected readonly RecollectableContext _context;
        protected readonly IPropertyMappingService _propertyMappingService;
        protected readonly IUnitOfWork _unitOfWork;

        public RecollectableTestBase()
        {
            var options = new DbContextOptionsBuilder<RecollectableContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RecollectableContext(options);
            _propertyMappingService = new PropertyMappingService();
            _unitOfWork = new UnitOfWork(_context, _propertyMappingService);
            RecollectableInitializer.Initialize(_context);
        }
    }
}
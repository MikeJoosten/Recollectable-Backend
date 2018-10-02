using Microsoft.EntityFrameworkCore;
using Recollectable.API.Services;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Infrastructure.Data;
using System;

namespace Recollectable.Tests
{
    public class RecollectableTestBase
    {
        private RecollectableContext _context;
        protected readonly IUnitOfWork _unitOfWork;
        private IPropertyMappingService _propertyMappingService;

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
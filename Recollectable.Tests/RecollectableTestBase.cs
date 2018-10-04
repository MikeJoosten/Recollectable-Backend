using Microsoft.EntityFrameworkCore;
using Moq;
using Recollectable.API.Interfaces;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Infrastructure.Data;
using System;

namespace Recollectable.Tests
{
    public class RecollectableTestBase
    {
        private readonly RecollectableContext _context;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly Mock<IPropertyMappingService> _mockPropertyMappingService;
        protected readonly Mock<ITypeHelperService> _mockTypeHelperService;
        protected readonly Mock<IControllerService> _mockControllerService;

        public RecollectableTestBase()
        {
            var options = new DbContextOptionsBuilder<RecollectableContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RecollectableContext(options);
            _mockPropertyMappingService = new Mock<IPropertyMappingService>();
            _unitOfWork = new UnitOfWork(_context, _mockPropertyMappingService.Object);

            _mockControllerService = new Mock<IControllerService>();
            _mockTypeHelperService = new Mock<ITypeHelperService>();
            _mockControllerService.SetupGet(x => x.TypeHelperService).Returns(_mockTypeHelperService.Object);
            _mockControllerService.SetupGet(x => x.PropertyMappingService).Returns(_mockPropertyMappingService.Object);

            RecollectableInitializer.Initialize(_context);
        }
    }
}
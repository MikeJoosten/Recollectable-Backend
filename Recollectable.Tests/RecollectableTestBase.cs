using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Recollectable.API.Interfaces;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Infrastructure.Data;
using System;
using System.Collections.Generic;

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
            _mockControllerService.SetupGet(c => c.TypeHelperService).Returns(_mockTypeHelperService.Object);
            _mockControllerService.SetupGet(c => c.PropertyMappingService).Returns(_mockPropertyMappingService.Object);

            var configuration = new MapperConfiguration(cfg =>
                cfg.AddProfile<RecollectableMappingProfile>());
            IMapper mapper = configuration.CreateMapper();
            _mockControllerService.SetupGet(c => c.Mapper).Returns(mapper);

            RecollectableInitializer.Initialize(_context);
        }

        public void SetupTestController<T, S>(Controller controller, 
            Dictionary<string, PropertyMappingValue> _propertyMapping)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("http://localhost/");

            controller.Url = mockUrlHelper.Object;

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(), 
                It.IsAny<ValidationStateDictionary>(), It.IsAny<string>(), It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;

            _mockTypeHelperService.Setup(t =>
                t.TypeHasProperties<T>(It.IsAny<string>())).Returns(true);
            _mockPropertyMappingService.Setup(p =>
                p.ValidMappingExistsFor<T, S>(It.IsAny<string>())).Returns(true);
            _mockPropertyMappingService.Setup(p =>
                p.GetPropertyMapping<T, S>()).Returns(_propertyMapping);
        }
    }
}
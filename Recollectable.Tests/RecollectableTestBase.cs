using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Recollectable.API.Interfaces;
using Recollectable.API.Services;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Infrastructure.Data;
using System;

namespace Recollectable.Tests
{
    public class RecollectableTestBase
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IPropertyMappingService _propertyMappingService;
        protected readonly ITypeHelperService _typeHelperService;
        protected readonly IMapper _mapper;

        public RecollectableTestBase()
        {
            var options = new DbContextOptionsBuilder<RecollectableContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var _context = new RecollectableContext(options);
            _propertyMappingService = new PropertyMappingService();
            _unitOfWork = new UnitOfWork(_context, _propertyMappingService);

            _typeHelperService = new TypeHelperService();

            var configuration = new MapperConfiguration(cfg =>
                cfg.AddProfile<RecollectableMappingProfile>());
            _mapper = configuration.CreateMapper();

            RecollectableInitializer.Initialize(_context);
        }

        public void SetupTestController<T, S>(Controller controller)
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
        }
    }
}
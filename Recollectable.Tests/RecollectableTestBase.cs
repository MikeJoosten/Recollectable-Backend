using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Recollectable.API.Filters;
using Recollectable.Infrastructure.Data;
using System;

namespace Recollectable.Tests
{
    public class RecollectableTestBase
    {
        protected readonly IMapper _mapper;

        public RecollectableTestBase()
        {
            var options = new DbContextOptionsBuilder<RecollectableContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var _context = new RecollectableContext(options);

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
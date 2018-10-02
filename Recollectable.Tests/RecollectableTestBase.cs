using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
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
        private readonly RecollectableContext _context;
        protected readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelper _urlHelper;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IPropertyMappingService _propertyMappingService;
        protected readonly IControllerService _controllerService;

        public RecollectableTestBase()
        {
            var options = new DbContextOptionsBuilder<RecollectableContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RecollectableContext(options);
            _propertyMappingService = new PropertyMappingService();
            _unitOfWork = new UnitOfWork(_context, _propertyMappingService);

            _urlHelper = new UrlHelper(new ActionContext());
            _typeHelperService = new TypeHelperService();
            _controllerService = new ControllerService(_urlHelper, 
                _typeHelperService, _propertyMappingService);

            RecollectableInitializer.Initialize(_context);
        }
    }
}
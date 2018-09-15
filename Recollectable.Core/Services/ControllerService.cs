using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Core.Services
{
    public class ControllerService : IControllerService
    {
        public IUrlHelper UrlHelper { get; }
        public ITypeHelperService TypeHelperService { get; }
        public IPropertyMappingService PropertyMappingService { get; }

        public ControllerService(IUrlHelper urlHelper, ITypeHelperService typeHelperService,
            IPropertyMappingService propertyMappingService)
        {
            UrlHelper = urlHelper;
            TypeHelperService = typeHelperService;
            PropertyMappingService = propertyMappingService;
        }
    }
}
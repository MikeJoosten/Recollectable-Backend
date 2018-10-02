using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Interfaces;
using Recollectable.Core.Shared.Interfaces;

namespace Recollectable.API.Services
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
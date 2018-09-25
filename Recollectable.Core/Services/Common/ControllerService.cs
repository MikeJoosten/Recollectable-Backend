using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Interfaces.Services;
using Recollectable.Core.Shared.Interfaces;

namespace Recollectable.Core.Services.Common
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
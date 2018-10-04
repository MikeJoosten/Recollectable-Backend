using Recollectable.API.Interfaces;
using Recollectable.Core.Shared.Interfaces;

namespace Recollectable.API.Services
{
    public class ControllerService : IControllerService
    {
        public ITypeHelperService TypeHelperService { get; }
        public IPropertyMappingService PropertyMappingService { get; }

        public ControllerService(ITypeHelperService typeHelperService,
            IPropertyMappingService propertyMappingService)
        {
            TypeHelperService = typeHelperService;
            PropertyMappingService = propertyMappingService;
        }
    }
}
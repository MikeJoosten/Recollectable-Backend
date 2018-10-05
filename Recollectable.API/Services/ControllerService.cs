using AutoMapper;
using Recollectable.API.Interfaces;
using Recollectable.Core.Shared.Interfaces;

namespace Recollectable.API.Services
{
    public class ControllerService : IControllerService
    {
        public IMapper Mapper { get; }
        public ITypeHelperService TypeHelperService { get; }
        public IPropertyMappingService PropertyMappingService { get; }

        public ControllerService(ITypeHelperService typeHelperService,
            IPropertyMappingService propertyMappingService, IMapper mapper)
        {
            Mapper = mapper;
            TypeHelperService = typeHelperService;
            PropertyMappingService = propertyMappingService;
        }
    }
}
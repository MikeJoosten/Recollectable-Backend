using AutoMapper;
using Recollectable.Core.Shared.Interfaces;

namespace Recollectable.API.Interfaces
{
    public interface IControllerService
    {
        IMapper Mapper { get; }
        ITypeHelperService TypeHelperService { get; }
        IPropertyMappingService PropertyMappingService { get; }
    }
}
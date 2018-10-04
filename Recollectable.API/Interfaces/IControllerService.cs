using Recollectable.Core.Shared.Interfaces;

namespace Recollectable.API.Interfaces
{
    public interface IControllerService
    {
        ITypeHelperService TypeHelperService { get; }
        IPropertyMappingService PropertyMappingService { get; }
    }
}
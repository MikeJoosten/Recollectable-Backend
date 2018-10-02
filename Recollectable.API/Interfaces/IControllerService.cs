using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Shared.Interfaces;

namespace Recollectable.API.Interfaces
{
    public interface IControllerService
    {
        IUrlHelper UrlHelper { get; }
        ITypeHelperService TypeHelperService { get; }
        IPropertyMappingService PropertyMappingService { get; }
    }
}
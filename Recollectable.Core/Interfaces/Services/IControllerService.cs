using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Shared.Interfaces;

namespace Recollectable.Core.Interfaces.Services
{
    public interface IControllerService
    {
        IUrlHelper UrlHelper { get; }
        ITypeHelperService TypeHelperService { get; }
        IPropertyMappingService PropertyMappingService { get; }
    }
}
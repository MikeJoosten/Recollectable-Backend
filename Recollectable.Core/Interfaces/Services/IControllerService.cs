using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Core.Interfaces.Services
{
    public interface IControllerService
    {
        IUrlHelper UrlHelper { get; }
        ITypeHelperService TypeHelperService { get; }
        IPropertyMappingService PropertyMappingService { get; }
    }
}
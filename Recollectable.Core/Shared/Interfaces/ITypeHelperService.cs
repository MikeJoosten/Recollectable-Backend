namespace Recollectable.Core.Shared.Interfaces
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
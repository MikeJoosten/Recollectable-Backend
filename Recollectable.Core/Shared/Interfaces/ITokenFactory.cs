using System.Threading.Tasks;

namespace Recollectable.Core.Shared.Interfaces
{
    public interface ITokenFactory
    {
        Task<string> GenerateToken(string userName);
    }
}
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface IEmailService
    {
        Task Send(string recipient, string subject, string message);
    }
}
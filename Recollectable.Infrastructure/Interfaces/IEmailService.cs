using Recollectable.Core.Shared.Enums;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task Send(string recipient, string subject, string message, MailType type);
    }
}
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Recollectable.Core.Shared.Entities
{
    public class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> 
        where TUser : class
    {
        public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<EmailConfirmationTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger) 
            : base(dataProtectionProvider, options, logger)
        { }
    }
}
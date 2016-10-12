﻿using ServiceBase.IdentityServer.Models;
using ServiceBase.Notification.Email;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Extensions
{
    public static class IEmailServiceExtensions
    {
        public async static Task SendAccountCreatedEmailAsync(this IEmailService emailService, UserAccount userAccount)
        {
            await emailService.SendEmailAsync("AccountCreatedEvent", userAccount.Email, new
            {
                Token = userAccount.VerificationKey
            });
        }
    }
}

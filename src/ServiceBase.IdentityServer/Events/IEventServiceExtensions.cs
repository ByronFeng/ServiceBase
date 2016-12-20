﻿using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;

namespace ServiceBase.IdentityServer.Events
{
    public static class IEventServiceExtensions
    {
        /// <summary>
        /// Raises successfull <see cref="Models.UserAccount" created <see cref="IdentityServer4.Events.Event{UserAccountCreatedDetails}"/> />
        /// </summary>
        /// <param name="events"></param>
        /// <param name="userAccountId"><see cref="Models.UserAccount"/> primary key</param>
        /// <param name="provider">Used Identity Provider to register the <see cref="Models.UserAccount"/></param>
        /// <returns></returns>
        public static async Task RaiseSuccessfulUserAccountCreatedEventAsync(this IEventService events, Guid userAccountId, string provider)
        {
            var evt = new Event<UserAccountCreatedDetails>(
                EventConstants.Categories.UserAccount,
                "User Account Creation Success",
                EventTypes.Success,
                EventConstants.Ids.UserAccountCreated,
                new UserAccountCreatedDetails
                {
                    UserAccountId = userAccountId,
                    Provider = provider
                });

            await events.RaiseAsync(evt);
        }

        /// <summary>
        /// Raises successfull <see cref="Models.UserAccount" updated <see cref="IdentityServer4.Events.Event{UserAccountUpdatedDetails}"/> />
        /// </summary>
        /// <param name="events"></param>
        /// <param name="userAccountId"><see cref="Models.UserAccount"/> primary key</param>
        /// <returns></returns>
        public static async Task RaiseSuccessfulUserAccountUpdatedEventAsync(this IEventService events, Guid userAccountId)
        {
            var evt = new Event<UserAccountUpdatedDetails>(
                 EventConstants.Categories.UserAccount,
                 "User Account Update Success",
                 EventTypes.Success,
                 EventConstants.Ids.UserAccountUpdated,
                 new UserAccountUpdatedDetails
                 {
                     UserAccountId = userAccountId
                 });

            await events.RaiseAsync(evt);

        }
    }
}

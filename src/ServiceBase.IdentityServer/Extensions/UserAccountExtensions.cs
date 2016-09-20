﻿using Host.Models;
using System;

namespace Host.Extensions
{
    public static class UserAccountExtensions
    {
        public static bool HasPassword(this UserAccount userAccount)
        {
            if (userAccount == null) throw new ArgumentException(nameof(userAccount));

            return !String.IsNullOrWhiteSpace(userAccount.PasswordHash);
        }

        public static bool IsNew(this UserAccount userAccount)
        {
            if (userAccount == null) throw new ArgumentException(nameof(userAccount));

            return !userAccount.LastLoginAt.HasValue;
        }

        public static void SetVerification(this UserAccount userAccount,
            string key,
            VerificationKeyPurpose purpose,
            string storage = null,
            DateTime? sentAt = null)
        {
            if (userAccount == null) throw new ArgumentException(nameof(userAccount));
            if (key == null) throw new ArgumentException(nameof(key));

            userAccount.VerificationKey = key;
            userAccount.VerificationPurpose = (int)purpose;
            userAccount.VerificationKeySentAt = sentAt ?? DateTime.UtcNow;
            userAccount.VerificationStorage = storage;
        }

        public static void ClearVerification(this UserAccount userAccount)
        {
            userAccount.VerificationKey = null;
            userAccount.VerificationPurpose = null;
            userAccount.VerificationKeySentAt = null;
            userAccount.VerificationStorage = null;
        }
    }
}

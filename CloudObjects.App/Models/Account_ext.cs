﻿using AO.Models;
using AO.Models.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CloudObjects.Models
{
    public partial class Account : IValidate
    {
        public override bool TrackDeletions => false;

        public ValidateResult Validate()
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQURSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.-";

            if (Name.Length < 5) return new ValidateResult("Account name must be least 5 characters long.");

            var illegalChars = Name.ToCharArray().Where(c => !allowedChars.Contains(c)).ToArray();
            if (illegalChars.Any())
            {
                return new ValidateResult($"Account name {Name} contains one more illegal characters: {string.Join(", ", illegalChars)}");
            }

            return new ValidateResult();
        }

        public async Task<ValidateResult> ValidateAsync(IDbConnection connection, IDbTransaction txn = null)
        {
            return await Task.FromResult(new ValidateResult());
        }
    }
}

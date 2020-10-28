using AO.Models;
using AO.Models.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CloudObjects.Models
{
    public partial class StoredObject : IValidate
    {
        [JsonIgnore]
        public override bool TrackDeletions => true;

        public ValidateResult Validate()
        {
            var specialChars = "?!@#$%^'|&*()+=;\"".ToCharArray();

            return (Uri.IsWellFormedUriString(Name, UriKind.Relative) && !ContainsAny(Name, specialChars)) ? 
                ValidateResult.Ok() : 
                ValidateResult.Failed($"Object Name must be well-formed URI string, and may not contain these characters: {string.Join(", ", specialChars)}.");
        }

        private bool ContainsAny(string name, char[] charSet) => name.ToCharArray().Any(c => charSet.Contains(c));
        
        public async Task<ValidateResult> ValidateAsync(IDbConnection connection, IDbTransaction txn = null) => await ValidateResult.OkAsync();
        
    }
}

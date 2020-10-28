using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CloudObjects.App.Extensions
{
    public static class ControllerExtensions
    {
        public static T GetClaim<T>(this ControllerBase controller, string claimName, Func<string, T> convert, Func<IEnumerable<Claim>, Claim> claimSelector = null)
        {
            // claims aren't unique by type, but grouping them by type is a good first step to finding one we want
            var claimsLookup = controller.User.Claims.ToLookup(item => item.Type);

            // how do we pick from possible duplicate claim types?
            // in the absence of an explicit rule, we'll simply take the first
            if (claimSelector == null) claimSelector = (claims) => claims.First();

            // get the desired claim value, resolving any duplicate according to our selector rule
            var claimValue = claimSelector.Invoke(claimsLookup[claimName]).Value;

            // apply the type conversion to get our result in the type we need it to be
            return convert.Invoke(claimValue);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudObjects.App.Extensions
{
    public static class ConnectionString
    {
        public static string Token(string input, string token, string defaultValue = default)
        {
            var dictionary = ToDictionary(input);

            return dictionary.ContainsKey(token) ? dictionary[token] : defaultValue;
        }        

        public static string Token(string input, IEnumerable<string> tokens, string defaultValue = default)
        {
            var dictionary = ToDictionary(input);

            var key = tokens.FirstOrDefault(item => dictionary.ContainsKey(item));

            return (key != default) ? dictionary[key] : defaultValue;
        }

        private static Dictionary<string, string> ToDictionary(string input) => 
            input
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item =>
                {
                    var parts = item.Split('=');
                    return new
                    {
                        Name = parts[0],
                        Value = parts[1]
                    };
                }).ToDictionary(item => item.Name, item => item.Value);

        public static string Redact(string input)
        {
            return (IsSensitive(input, out Dictionary<string, string> dictionary)) ?
                string.Join(";", dictionary.Select(kp => $"{kp.Key}={kp.Value}")) :
                input;
        }

        public static bool IsSensitive(string input, out Dictionary<string, string> redacted)
        {
            try
            {
                bool result = false;
                var original = ToDictionary(input);
                redacted = ToDictionary(input);      

                foreach (var kp in original)
                {
                    if (kp.Key.ToLower().Equals("password") || kp.Key.ToLower().Equals("user id"))
                    {
                        result = true;
                        redacted[kp.Key] = "&lt;redacted&gt;";
                    }                    
                }

                return result;
            }
            catch 
            {
                redacted = null;
                return false;
            }
        }
    }
}

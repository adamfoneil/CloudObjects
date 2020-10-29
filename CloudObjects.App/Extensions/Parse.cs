using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudObjects.App.Extensions
{
    public static class Parse
    {
        public static string Token(string input, string token, char separator = ';', string defaultValue = default)
        {
            var dictionary = ToDictionary(input, separator);

            return dictionary.ContainsKey(token) ? dictionary[token] : defaultValue;
        }        

        public static string Token(string input, IEnumerable<string> tokens, char separator = ';', string defaultValue = default)
        {
            var dictionary = ToDictionary(input, separator);

            var key = tokens.FirstOrDefault(item => dictionary.ContainsKey(item));

            return (key != default) ? dictionary[key] : defaultValue;
        }

        private static Dictionary<string, string> ToDictionary(string input, char separator) => 
            input
                .Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item =>
                {
                    var parts = item.Split('=');
                    return new
                    {
                        Name = parts[0],
                        Value = parts[1]
                    };
                }).ToDictionary(item => item.Name, item => item.Value);
    }
}

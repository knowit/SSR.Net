using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SSR.Net.Extensions
{
    public static class StringExtensions
    {
        static readonly Regex ToEscape = new Regex("<|>", RegexOptions.Compiled);
        public static string SanitizeInitScript(this string script)
        {
            var replacements = new Dictionary<string, string>
                                    {
                                        { "<", "\\u003c" },
                                        { ">", "\\u003e" }
                                    };
            return ToEscape.Replace(script, match => replacements[match.Value]);
        }
    }
}

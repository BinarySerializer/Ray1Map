using System;

namespace R1Engine
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);

            if (pos < 0)
                return text;
            
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
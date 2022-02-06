using System;

namespace config.utils
{
    public static class StringHelper
    {
        public static bool EqualsIgnoreCase(this string text1, string text2) => String.Equals(text1, text2, StringComparison.CurrentCultureIgnoreCase);
    }
}
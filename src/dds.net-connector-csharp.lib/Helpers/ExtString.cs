using System.Text.RegularExpressions;

namespace DDS.Net.Connector.Helpers
{
    internal static class ExtString
    {
        private static Regex spacesPattern = new(@"\s+");

        internal static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ||
                   string.IsNullOrWhiteSpace(value);
        }

        internal static string RemoveSpaces(this string value)
        {
            return spacesPattern.Replace(value, "");
        }

        internal static bool ContainsAnyIgnoringCase(this string text,
            string text01,
            string text02)
        {
            return text.Contains(text01, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text02, StringComparison.CurrentCultureIgnoreCase);
        }

        internal static bool ContainsAnyIgnoringCase(this string text,
            string text01,
            string text02,
            string text03)
        {
            return text.Contains(text01, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text02, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text03, StringComparison.CurrentCultureIgnoreCase);
        }

        internal static bool ContainsAnyIgnoringCase(this string text,
            string text01,
            string text02,
            string text03,
            string text04)
        {
            return text.Contains(text01, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text02, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text03, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text04, StringComparison.CurrentCultureIgnoreCase);
        }

        internal static bool ContainsAnyIgnoringCase(this string text,
            string text01,
            string text02,
            string text03,
            string text04,
            string text05)
        {
            return text.Contains(text01, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text02, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text03, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text04, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text05, StringComparison.CurrentCultureIgnoreCase);
        }

        internal static bool ContainsAnyIgnoringCase(this string text,
            string text01,
            string text02,
            string text03,
            string text04,
            string text05,
            params string[] textContd)
        {
            bool contains =
                   text.Contains(text01, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text02, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text03, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text04, StringComparison.CurrentCultureIgnoreCase) ||
                   text.Contains(text05, StringComparison.CurrentCultureIgnoreCase);

            if (contains)
            {
                return true;
            }
            else
            {
                foreach (var item in textContd)
                {
                    if (text.Contains(item, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

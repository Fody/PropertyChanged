using System.Text.RegularExpressions;

static class Extensions
{
    public static bool ContainsWholeWord(this string str, string word)
        => Regex.IsMatch(str, @"\b" + Regex.Escape(word) + @"\b", RegexOptions.CultureInvariant);
}

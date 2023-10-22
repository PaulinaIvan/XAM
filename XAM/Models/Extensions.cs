public static class Extensions
{
    // Returns true if string contains only letters and spaces.
    public static bool IsMadeOfLettersNumbersAndSpaces(this string str)
    {
        return !string.IsNullOrEmpty(str) && str.All(c => char.IsLetterOrDigit(c) || c == ' ');
    }
}
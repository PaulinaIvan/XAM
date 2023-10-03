public static class Extensions
{
    // Returns true if string contains only letters and spaces.
    public static bool IsMadeOfLettersNumbersAndSpaces(this string str)
    {
        if(str == null)
            return false;

        foreach(char c in str)
        {
            if(!char.IsLetter(c) && !char.IsNumber(c) && c != ' ')
                return false;
        }

        return true;
    }
}
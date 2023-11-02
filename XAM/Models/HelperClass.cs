using System.Text.RegularExpressions;

namespace XAM.Models;

public static class HelperClass
{
    // Returns true if string contains only letters and spaces.
    public static bool IsMadeOfLettersNumbersAndSpaces(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        string pattern = @"^[\p{L}\p{N} ]+$"; // Letters and numbers from any language, and spaces pattern (at least one character)
        return Regex.IsMatch(str, pattern); // 9. Regex usage
    }

    // For easier error messaging to frontend.
    public record ErrorRecord(string ErrorCode, string ErrorMessage);

    public static ErrorRecord CreateErrorResponse(string ErrorCode, string ErrorMessage = "Unknown error.")
    {
        ErrorRecord ErrorResponse = new(ErrorCode, ErrorMessage);
        return ErrorResponse;
    }
}
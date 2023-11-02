namespace XAM.Models;

public static class HelperClass
{
    // Returns true if string contains only letters and spaces.
    public static bool IsMadeOfLettersNumbersAndSpaces(this string str)
    {
        return !string.IsNullOrEmpty(str) && str.All(c => char.IsLetterOrDigit(c) || c == ' ');
    }

    // For easier error messaging to frontend.
    public record ErrorRecord(string ErrorCode, string ErrorMessage);

    public static ErrorRecord CreateErrorResponse(string ErrorCode, string ErrorMessage = "Unknown error.")
    {
        ErrorRecord ErrorResponse = new(ErrorCode, ErrorMessage);
        return ErrorResponse;
    }
}
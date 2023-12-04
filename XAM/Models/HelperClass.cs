using System.Text.RegularExpressions;

namespace XAM.Models;

public static class HelperClass
{
    // Returns true if string contains only letters and spaces.
    public static bool IsValidExamName(this string str)
    {
        try
        {
            if (string.IsNullOrEmpty(str))
                throw new InvalidExamNameException("Null or empty exam name found in backend!");

            string pattern = @"^[\p{L}\p{N} ]+$"; // Letters and numbers from any language, and spaces pattern (at least one character)
            return Regex.IsMatch(str, pattern);
        }
        catch (InvalidExamNameException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    // For easier error messaging to frontend.
    public record ErrorRecord(string ErrorCode, string ErrorMessage);

    public static ErrorRecord CreateErrorResponse(string ErrorCode, string ErrorMessage = "Unknown error.")
    {
        ErrorRecord ErrorResponse = new(ErrorCode, ErrorMessage);
        return ErrorResponse;
    }
}
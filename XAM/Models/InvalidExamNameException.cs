namespace XAM.Models;

public class InvalidExamNameException : Exception
{
    public InvalidExamNameException() { }

    public InvalidExamNameException(string message) : base(message) { }

    public InvalidExamNameException(string message, Exception innerException) : base(message, innerException) { }
}


namespace XAM.Models;

public class APIRequestExeption : Exception
{
    public APIRequestExeption() { }

    public APIRequestExeption(string message) : base(message) { }

    public APIRequestExeption(string message, Exception innerException) : base(message, innerException) { }
}

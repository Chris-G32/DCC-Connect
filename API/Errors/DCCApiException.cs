namespace API.Errors;

/// <summary>
/// Base class for all of our custom errors to inherit from. Useful for identifying wrrors that are part of desired behavior and those that could indicate bugs.
/// </summary>
public class DCCApiException:Exception
{
    public DCCApiException(string message) : base(message)
    {
    }
    public DCCApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

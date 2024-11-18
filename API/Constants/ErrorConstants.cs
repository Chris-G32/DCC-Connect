namespace API.Constants;

public class ErrorConstants
{
    public static string ObjectDoesNotExistError { get; } = "Failed to find object with ID";
    public static string ErrorInMongoDB { get; } = "Error in database, retry request.";
}

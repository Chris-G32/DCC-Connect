namespace API.Constants;

public class ValidationConstants
{
    public static int MaxLocationStringLength { get; } = 250;

    // According to docs as of 2024 this is the max length an object id will be in mongo
    public static int MaxObjectIdStringLength { get; } = 24;
}

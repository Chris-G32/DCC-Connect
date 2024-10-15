namespace API.Utils;

public class ErrorUtils
{
    public static string FormatObjectDoesNotExistErrorString(string ID) { return $"Failed to find object with ID {ID}"; }
    public static string FormatObjectDoesNotExistErrorString(string ID, string collectionName) { return $"{FormatObjectDoesNotExistErrorString(ID)} in collection {collectionName}"; }
}

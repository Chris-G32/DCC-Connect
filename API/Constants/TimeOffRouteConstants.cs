namespace API.Constants;

public class TimeOffRouteConstants
{
    public static string TimeOffBase { get; } = "timeoff/";
    public static string RequestTimeOffRoute { get; } = TimeOffBase + "request/";
    public static string CancelTimeOffRoute { get; } = TimeOffBase + "cancel/";
    public static string GetTimeOffRequestsRoute { get; } = TimeOffBase + "get/";
}

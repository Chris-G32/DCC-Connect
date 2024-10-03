namespace API.Constants;

public class RouteConstants
{
    //Shift Scheduling Routes
    public static string ShiftRouteBase { get; } = "shifts/";
    public static string CreateShiftRoute { get; } = ShiftRouteBase + "create/";
    public static string DeleteShiftRoute { get; } = ShiftRouteBase + "delete/";
    public static string AssignShiftRoute { get; } = ShiftRouteBase + "assign/";
    public static string UnassignShiftRoute { get; } = ShiftRouteBase + "unassign/";

    //Employee Interaction Routes
    public static string EmployeeRouteBase { get; } = "employees/";
    public static string OfferUpShiftRoute { get; } = EmployeeRouteBase + "offerup/";
    public static string CancelOfferUpShiftRoute { get; } = OfferUpShiftRoute + "cancel/";
    public static string PickUpShiftRoute { get; } = EmployeeRouteBase + "pickup/";

}

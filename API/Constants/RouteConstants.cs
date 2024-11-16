namespace API.Constants;

public class RouteConstants
{
    //Shift Scheduling Routes
    public static string ShiftRouteBase { get; } = "shifts/";
    public static string CreateShiftRoute { get; } = ShiftRouteBase + "create/";
    public static string DeleteShiftRoute { get; } = ShiftRouteBase + "delete/";
    public static string AssignShiftRoute { get; } = ShiftRouteBase + "assign/";
    public static string UnassignShiftRoute { get; } = ShiftRouteBase + "unassign/";

    //Shift Trading and Pickup Routes
    public static string EmployeeRouteBase { get; } = "employees/";
    public static string OfferUpShiftRoute { get; } = EmployeeRouteBase + "offerup/";
    public static string CancelOfferUpShiftRoute { get; } = OfferUpShiftRoute + "cancel/";
    public static string TradeShiftRoute { get; } = EmployeeRouteBase + "trade/";
    public static string PickUpShiftRoute { get; } = EmployeeRouteBase + "pickup/";
    // Approval Routes for trading and pickup
    public static string ApproveTradeRoute { get; } = TradeShiftRoute + "approve/";
    public static string DenyTradeRoute { get; } = TradeShiftRoute + "deny/";
    public static string ApprovePickupRoute { get; } = PickUpShiftRoute + "approve/";
    public static string DenyPickupRoute { get; } = PickUpShiftRoute + "deny/";

    //Query Routes
    public static string GetEmployeeRoute { get; } = EmployeeRouteBase + "get/";
    public static string GetShiftRoute { get; } = ShiftRouteBase + "get/";
    public static string GetOpenShiftRoute { get; } = GetShiftRoute + "open/";


}

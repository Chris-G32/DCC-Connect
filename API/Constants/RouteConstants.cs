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

    public static string CoverageRequestRouteBase { get; } = "coverage/";
    //Query Routes
    public static string GetEmployeeRoute { get; } = EmployeeRouteBase + "get/";
    public static string GetEmployeeByIdRoute { get; } = GetEmployeeRoute + "{id}/";
    public static string GetShiftRoute { get; } = ShiftRouteBase + "get/";
    public static string GetShiftByIdRoute { get; } = GetShiftRoute + "{id}/";
    public static string GetOpenShiftRoute { get; } = GetShiftRoute + "open/";
    public static string GetCoverageRequestRoute { get; } = CoverageRequestRouteBase + "get/";
    public static string GetCoverageRequestByIdRoute { get; } = GetCoverageRequestRoute + "{id}/";


    /* Auth Constants */
    public const string GenerateTokenRoute = "/auth/token";
    public const string ValidateTokenRoute = "/auth/validate";

    /* Email Constants */
    public const string Send2FACodeRoute = "/email/send2fa";
    public const string Validate2FACodeRoute = "/email/validate2fa";
    public const string SendPasswordResetRoute = "/email/sendpasswordreset";
    public const string ResetPasswordRoute = "/email/resetpassword";

    /* User Constants */
    public const string CreateUserRoute = "/user/create";
    public const string UpdateUserPasswordRoute = "/user/updatepassword";
    public const string RegisterUserRoute = "/user/register";
    public const string GetUserRoute = "/user";
    public const string UpdateUserRoute = "/user";
    public const string DeleteUserRoute = "/user";
    public const string LoginUserRoute = "/user/login";

}

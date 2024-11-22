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
    public static string GenerateTokenRoute { get; } = "/auth/token";
    public static string ValidateTokenRoute { get; } = "/auth/validate";

    /* Email Constants */
    public static string Send2FACodeRoute { get; } = "/email/send2fa";
    public static string Validate2FACodeRoute { get; } = "/email/validate2fa";
    public static string SendPasswordResetRoute { get; } = "/email/sendpasswordreset";
    public static string ResetPasswordRoute { get; } = "/email/resetpassword";

    /* User Constants */
    public static string UserRouteBase { get; } = "user/";
    public static string CreateUserRoute { get; } = UserRouteBase + "create/";
    public static string UpdateUserPasswordRoute { get; } = UserRouteBase + "updatepassword/";
    public static string RegisterUserRoute { get; } = UserRouteBase + "register/";
    public static string GetUserRoute { get; } = UserRouteBase;
    public static string GetUserRoleRoute = GetUserRoute + "role/{emailOrId}";
    public static string UpdateUserRoute { get; } = UserRouteBase;
    public static string DeleteUserRoute { get; } = UserRouteBase;
    public static string LoginUserRoute { get; } = UserRouteBase + "login/";
    public static string UpdateJWTTokenRoute { get; } = UserRouteBase + "updatejwt/";  // New route to update JWT token


}

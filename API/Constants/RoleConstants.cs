namespace API.Constants;

public class RoleConstants
{

    public static string Employee { get; } = "Employee";
    public static string HouseLead { get; } = "HouseLead";
    public static string HouseManager { get; } = "HouseManager";
    public static string Manager { get; } = "Manager";
    public static readonly HashSet<string> ValidRoles = new HashSet<string>
        {
            Employee,
            HouseLead,
            HouseManager,
            Manager
    };
}

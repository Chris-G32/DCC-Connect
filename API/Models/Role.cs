using API.Constants;

namespace API.Models;

public enum Role
{
    Employee,
    HouseLead,
    HouseManager,
    Manager
}
public class RoleConversions
{
    public static string ToString(Role role)
    {
        switch (role)
        {
            case Role.Employee:
                return RoleConstants.Employee;
            case Role.HouseLead:
                return RoleConstants.HouseLead;
            case Role.HouseManager:
                return RoleConstants.HouseManager;
            case Role.Manager:
                return RoleConstants.Manager;
            default:
                throw new ArgumentException("Invalid role");
        }
    }
    public static Role FromString(string role)
    {

        if (role == RoleConstants.Employee)
        {
            return Role.Employee;
        }
        else if (role == RoleConstants.HouseLead)
        {
            return Role.HouseLead;
        }
        else if (role == RoleConstants.HouseManager)
        {
            return Role.HouseManager;
        }
        else if (role == RoleConstants.Manager)
        {
            return Role.Manager;
        }
        else
        {
            throw new ArgumentException("Invalid role", nameof(role));
        }
    }
}
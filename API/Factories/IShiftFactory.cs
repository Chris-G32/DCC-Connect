using API.Models;

namespace API.Factories;

public interface IShiftFactory
{
    Shift CreateShift(DateTime start, DateTime end, string location, string role);
}
public class ShiftFactory : IShiftFactory
{
    public Shift CreateShift(DateTime start, DateTime end, string location, string role)
    {
        throw new NotImplementedException();
    }
}
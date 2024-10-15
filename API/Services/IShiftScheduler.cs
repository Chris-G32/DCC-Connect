using API.Constants;
using API.Models;
using API.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services;


public interface IShiftScheduler
{
    void assignShift(ShiftAssignment assignment);
    void unassignShift(string shiftID);
    void createShift(Shift shift);
    void deleteShift(string shiftID);
}
public class ShiftScheduler(ICollectionsProvider collectionsProvider) : IShiftScheduler
{
    private readonly ICollectionsProvider _collectionsProvider = collectionsProvider;

    /// <summary>
    /// Performs database operations to update a shift assignment. Without regard for the validity of the assignment.
    /// </summary>
    /// <param name="assignment"></param>
    private void applyShiftAssignment(ShiftAssignment assignment)
    {
        UpdateDefinition<Shift> update = Builders<Shift>.Update.Set(shift => shift.EmployeeID, assignment.EmployeeID);
        _collectionsProvider.Shifts.UpdateOne(shift => shift.Id.ToString() == assignment.ShiftID, update);
    }
    public void assignShift(ShiftAssignment assignment)
    {
        // TODO: add overlapping check into here.
        // This is jut to enforce a more readable and intentional calling of the endpoint. Nothing in this code is in conflict with an unassignment of a shift being done
        if (string.IsNullOrEmpty(assignment.EmployeeID))
        {
            throw new Exception("EmployeeID null or empty. Please call unassign shift endpoint to remove an assignment.");
        }
        if (!DBEntityUtils.EntityWithIDExists(_collectionsProvider.Employees, assignment.EmployeeID))
        {
            throw new Exception($"Cannot Find employee with ID {assignment.EmployeeID}");
        }
        if (!DBEntityUtils.EntityWithIDExists(_collectionsProvider.Shifts, assignment.ShiftID))
        {
            throw new Exception($"Cannot Find shift with ID {assignment.ShiftID}");
        }
        applyShiftAssignment(assignment);
    }
    public void createShift(Shift shift)
    {
        _collectionsProvider.Shifts.InsertOne(shift);
    }

    public void deleteShift(string shiftID)
    {
        var shift = _collectionsProvider.Shifts.Find(shift => shift.Id.ToString() == shiftID).FirstOrDefault() ?? throw new Exception("Shift not found!");
        if (!string.IsNullOrEmpty(shift.EmployeeID))
        {
            throw new Exception("Please unassign a shift before deleting it.");
        }
        _collectionsProvider.Shifts.FindOneAndDelete(shift => shift.Id.ToString() == shiftID);
    }
    public void unassignShift(string shiftID)
    {
        if (!DBEntityUtils.EntityWithIDExists(_collectionsProvider.Shifts, shiftID))
        {
            throw new Exception($"Cannot Find shift with ID {shiftID}");
        }
        applyShiftAssignment(assignment: new ShiftAssignment
        {
            EmployeeID = null,
            ShiftID = shiftID
        });
    }
}
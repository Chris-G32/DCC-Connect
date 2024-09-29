using API.Constants;
using API.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services;

public interface IShiftScheduler
{
    void assignShift(ShiftAssignment assignment);
    void unassignShift(string shiftAssignmentID);
    void createShift(Shift shift);
    void deleteShift(string shiftID);
}
public class ShiftScheduler(ICollectionsProvider collectionsProvider) : IShiftScheduler
{
    private readonly ICollectionsProvider _collectionsProvider = collectionsProvider;
    public void assignShift(ShiftAssignment assignment)
    {
        if (_collectionsProvider.Employees.CountDocuments(e => e.Id.ToString() == assignment.EmployeeID) != 1)
        {
            throw new Exception($"Cannot Find employee with ID {assignment.EmployeeID}");
        }
        if (_collectionsProvider.Shifts.CountDocuments(s => s.Id.ToString() == assignment.ShiftID) != 1)
        {
            throw new Exception($"Cannot Find shift with ID {assignment.ShiftID}");
        }
        _collectionsProvider.AssignedShifts.InsertOne(assignment);
    }
    public void createShift(Shift shift)
    {
        _collectionsProvider.Shifts.InsertOne(shift);
    }

    public void deleteShift(string shiftID)
    {
        if(_collectionsProvider.AssignedShifts.Find(shift => shift.ShiftID.ToString() == shiftID).CountDocuments() > 0)
        {
            throw new Exception("Please unassign a shift before deleting it.");
        }
        _collectionsProvider.Shifts.FindOneAndDelete(shift => shift.Id.ToString() == shiftID);
    }
    public void unassignShift(string shiftAssignmentID)
    {
        _collectionsProvider.AssignedShifts.FindOneAndDelete(assignment=> assignment.Id.ToString()== shiftAssignmentID);
    }
}
using API.Constants;
using API.Models;
using API.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services;


public interface IShiftScheduler
{
    void AssignShift(ShiftAssignment assignment);
    void UnassignShift(string shiftID);
    void CreateShift(Shift shift);
    void DeleteShift(string shiftID);
}
public class ShiftScheduler(ICollectionsProvider collectionsProvider, IAvailabiltyService availabiltyService, IDBTransactionService transactService) : IShiftScheduler
{
    private readonly ICollectionsProvider _collectionsProvider = collectionsProvider;
    private readonly IAvailabiltyService _availabiltyService = availabiltyService;
    private readonly IDBTransactionService _transactService = transactService;

    /// <summary>
    /// Performs database operations to update a shift assignment. Without regard for the validity of the assignment.
    /// </summary>
    /// <param name="assignment"></param>
    private void ApplyShiftAssignment(ShiftAssignment assignment)
    {
        UpdateDefinition<Shift> update = Builders<Shift>.Update.Set(shift => shift.EmployeeID, assignment.EmployeeID);
        _collectionsProvider.Shifts.UpdateOne(shift => shift.Id.ToString() == assignment.ShiftID, update);
    }
    public void AssignShift(ShiftAssignment assignment)
    {
        DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Employees, assignment.EmployeeID);
        DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, assignment.ShiftID);

        if (!_availabiltyService.IsEmployeeSchedulableForShift(assignment.EmployeeID, assignment.ShiftID)) 
        {
            throw new Exception("Can't assign employee to shift.");
        }
        ApplyShiftAssignment(assignment);
        //TODO: Notify employee
    }
    public void CreateShift(Shift shift)
    {
        //TODO: Check the location exists.
        if (shift.ShiftPeriod.Start.ToUniversalTime() < DateTime.Now.ToUniversalTime())
        {
            throw new Exception("Cannot create a new shift in the past.");
        }
        _collectionsProvider.Shifts.InsertOne(shift);
    }
    public void DeleteShift(string shiftID)
    {
        var shift = DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, shiftID);
        if (!string.IsNullOrEmpty(shift.EmployeeID))
        {
            throw new Exception("Please unassign a shift before deleting it.");
        }
        _collectionsProvider.Shifts.FindOneAndDelete(shift => shift.Id.ToString() == shiftID);
    }
    public void UnassignShift(string shiftID)
    {
        var shift = DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, shiftID);
        if (string.IsNullOrEmpty(shift.EmployeeID))
        {
            return;
        }
        _transactService.RunTransaction(() =>
        {
            var coverage = _collectionsProvider.CoverageRequests.FindOneAndDelete(coverage => coverage.ShiftID.ToString() == shiftID);
            //Consider simply having a deny endpoint implemented somewheres
            _collectionsProvider.TradeOffers.DeleteMany(offer => offer.CoverageRequestID.ToString() == coverage.ShiftID);
            ApplyShiftAssignment(assignment: new ShiftAssignment
            {
                EmployeeID = null,
                ShiftID = shiftID
            });
        });
        //TODO: Notify employee
    }
}
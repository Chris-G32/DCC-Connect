using API.Constants;
using API.Models;
using API.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services;


public interface IShiftScheduler
{
    void AssignShift(ShiftAssignment assignment);
    void UnassignShift(ObjectId shiftID);
    void CreateShift(Shift shift);
    void DeleteShift(ObjectId shiftID);
}
public class ShiftScheduler(ILogger<ShiftScheduler> logger,IEntityRetriever entityRetriever, IAvailabiltyService availabiltyService) : IShiftScheduler
{
    private readonly IEntityRetriever _entityRetriever = entityRetriever;
    private readonly ICollectionsProvider _collectionsProvider = entityRetriever.CollectionsProvider;
    private readonly IAvailabiltyService _availabiltyService = availabiltyService;
    private readonly ILogger<ShiftScheduler> _logger = logger;

    /// <summary>
    /// Assigns an employee to a shift in the database
    /// </summary>
    /// <param name="assignment">The assignment to execute</param>
    /// <returns>True on successful assignment, false otherwise</returns>
    private bool ApplyShiftAssignment(ShiftAssignment assignment)
    {
        UpdateDefinition<Shift> update = Builders<Shift>.Update.Set(shift => shift.EmployeeID, assignment.EmployeeID);
        var result=_collectionsProvider.Shifts.UpdateOne(shift => shift.Id == assignment.ShiftID, update);
        if (result.ModifiedCount > 1)
        {
            _logger.LogCritical("Multiple shifts were modified by a single assignment.");
        }
        return result.ModifiedCount > 0;
    }
    public void AssignShift(ShiftAssignment assignment)
    {
        //DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Employees, assignment.EmployeeID);
        //DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, assignment.ShiftID);

        if (!_availabiltyService.IsEmployeeSchedulableForShift(assignment.EmployeeID, assignment.ShiftID))
        {
            throw new Exception("Can't assign employee to shift.");
        }
        if (!ApplyShiftAssignment(assignment))
        {
            _logger.LogInformation($"Failed to assign ");
            throw new Exception("Shift assignment failed.");
        }
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
    public void DeleteShift(ObjectId shiftID)
    {
        var shift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, shiftID);
        if (shift.EmployeeID != null)
        {
            throw new Exception("Please unassign a shift before deleting it.");
        }
        _collectionsProvider.Shifts.FindOneAndDelete(shift => shift.Id == shiftID);
    }
    public void UnassignShift(ObjectId shiftID)
    {
        var shift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, shiftID);
        if (shift.EmployeeID==null)
        {   
            return;
        }

        var coverage = _collectionsProvider.CoverageRequests.FindOneAndDelete(coverage => coverage.ShiftID == shiftID);
        if (coverage != null)
        {
            _collectionsProvider.TradeOffers.DeleteMany(offer => offer.CoverageRequestID == coverage.ShiftID);
        }

        ApplyShiftAssignment(assignment: new ShiftAssignment
        {
            EmployeeID = null,
            ShiftID = shiftID
        });

        //TODO: Notify employee
    }
}
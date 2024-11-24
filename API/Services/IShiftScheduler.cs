using API.Constants;
using API.Constants.Errors;
using API.Errors;
using API.Models.Shifts;
using API.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services;


public interface IShiftScheduler
{
    void AssignShift(ShiftAssignment assignment);
    void UnassignShift(ObjectId shiftID);
    void CreateShift(ShiftCreationInfo shift,out Shift createdShift);
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
            throw new DCCApiException(ShiftSchedulerErrorConstants.EmployeeNotSchedulableError);
        }
        if (!ApplyShiftAssignment(assignment))
        {
            _logger.LogInformation($"Failed to assign ");
            throw new DCCApiException(ShiftSchedulerErrorConstants.ShiftAssignmentUpdateFailedError);
        }
        //TODO: Notify employee
    }
    public void CreateShift(ShiftCreationInfo shiftCreation,out Shift createdShift)
    {
        
        //TODO: Check the location exists.
        if (shiftCreation.ShiftPeriod.Start.ToUniversalTime() < DateTime.Now.ToUniversalTime())
        {
            throw new DCCApiException(ShiftSchedulerErrorConstants.CannotCreateShiftInThePastError);
        }
        createdShift=new Shift(shiftCreation);
        createdShift.Id=ObjectId.GenerateNewId();
        _collectionsProvider.Shifts.InsertOne(createdShift);
    }
    public void DeleteShift(ObjectId shiftID)
    {
        var shift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, shiftID);
        if (shift.EmployeeID != null)
        {
            throw new DCCApiException(ShiftSchedulerErrorConstants.UnassignShiftBeforeDeleteError);
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

        if(!ApplyShiftAssignment(assignment: new ShiftAssignment
        {
            EmployeeID = null,
            ShiftID = shiftID
        }))
        {
            throw new DCCApiException(ShiftSchedulerErrorConstants.ShiftAssignmentUpdateFailedError);
        }

        //TODO: Notify employee
    }
}
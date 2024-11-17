using API.Errors;
using API.Models;
using API.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services;

/// <summary>
/// Provides information on availability of employees and of shifts.
/// </summary>
public interface IAvailabiltyService
{
    /// <summary>
    /// Get the availability of an employee
    /// </summary>
    /// <param name="employeeID">ID of the employee to get availability for</param>
    /// <param name="timeRange">Range to check availability</param>
    /// <returns>True if the employee is available for the entire range, false otherwise</returns>
    bool IsEmployeeAvailable(ObjectId? employeeID, TimeRange timeRange);
    bool IsKnownToExistEmployeeAvailable(ObjectId? employeeID, TimeRange timeRange);
    bool IsShiftOpen(ObjectId? shiftID, out Shift? shift);
    List<Employee> GetAvailableEmployeesForShift(ObjectId shiftID);
    List<Employee> GetAvailableEmployeesForShift(Shift shift);
    bool IsEmployeeSchedulableForShift(ObjectId? employeeID, ObjectId shiftID);
}
public class AvailablityService(IEntityRetriever entityRetriever) : IAvailabiltyService
{
    private readonly IEntityRetriever _entityRetriever = entityRetriever;
    private readonly ICollectionsProvider _collectionsProvider = entityRetriever.CollectionsProvider;
    /// <summary>
    /// Checks if a given employee is free to work a given shift. Returns false when employee or shift does not exist.
    /// </summary>
    /// <param name="employeeID"></param>
    /// <param name="shiftID"></param>
    /// <returns> True if an employee os available and a shift is open for scheduling. False when an ID is not found or an employee is not available.</returns>
    public bool IsEmployeeSchedulableForShift(ObjectId? employeeID, ObjectId shiftID)
    {
        if (employeeID == null)
        {
            return false;
        }

        var isOpen = IsShiftOpen(shiftID, out Shift? shift);
        if (shift == null)
        {
            return false;
        }
        var isEmployeeAvailable = IsEmployeeAvailable(employeeID, shift.ShiftPeriod);
        // TODO: Add check for time off
        return isOpen & isEmployeeAvailable;
    }
    public bool IsKnownToExistEmployeeAvailable(ObjectId? employeeID, TimeRange timeRange)
    {
        var builder = Builders<Shift>.Filter;
        var filter = builder.Eq(shift => shift.EmployeeID, employeeID) &
             builder.Lte(shift => shift.ShiftPeriod.Start, timeRange.End) &
             builder.Gte(shift => shift.ShiftPeriod.End, timeRange.Start);
        return _collectionsProvider.Shifts.CountDocuments(filter) < 1;
    }
    public bool IsEmployeeAvailable(ObjectId? employeeID, TimeRange timeRange)
    {

        if (employeeID == null)
        {
            return false;
        }

        if (!_entityRetriever.DoesEntityExist(_collectionsProvider.Employees, employeeID))
        {
            return false;
        }

        return IsKnownToExistEmployeeAvailable(employeeID, timeRange);
    }
    public List<Employee> GetAvailableEmployeesForShift(ObjectId shiftID)
    {
        return GetAvailableEmployeesForShift(_entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, shiftID));
    }
    public List<Employee> GetAvailableEmployeesForShift(Shift shift)
    {
        var shiftID = shift.Id ?? throw new DCCApiException("Shift has no ID populated.");
        return _collectionsProvider.Employees.Find(employee => IsKnownToExistEmployeeAvailable(employee.Id, shift.ShiftPeriod)).ToList();
    }
    /// <summary>
    /// Checks if a shift is schedulable. That is it is in the future, and either has not been assigned or is open for pickup.
    /// </summary>
    /// <param name="shiftID"></param>
    /// <param name="shift"></param>
    /// <returns></returns>
    public bool IsShiftOpen(ObjectId? shiftID, out Shift? shift)
    {
        shift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, shiftID);
        if (shift == null)
        {
            return false;
        }

        if (shift.EmployeeID == null)
        {
            return true;
        }
        if (shift.ShiftPeriod.Start < DateTime.Now || shift.ShiftPeriod.End < DateTime.Now)
        {
            return false;
        }
        var coverageRequest = _collectionsProvider.CoverageRequests.Find(coverage => coverage.ShiftID == shiftID).FirstOrDefault();
        if (coverageRequest == null)
        {
            return false;
        }
        return coverageRequest.CanPickup();
    }
}
using API.Models;
using API.Models.QueryOptions;
using API.Utils;
using Microsoft.Extensions.Options;
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
    bool IsEmployeeAvailable(string employeeID, TimeRange timeRange);
    bool IsShiftOpen(string shiftID, out Shift shift);
    bool IsEmployeeSchedulableForShift(string employeeID, string shiftID);
}
public class AvailablityService(ICollectionsProvider cp) : IAvailabiltyService
{
    private readonly ICollectionsProvider _collectionsProvider = cp;
    /// <summary>
    /// Checks if a given employee is free to work a given shift.
    /// </summary>
    /// <param name="employeeID"></param>
    /// <param name="shiftID"></param>
    /// <returns></returns>
    public bool IsEmployeeSchedulableForShift(string employeeID, string shiftID)
    {
        var shift = new Shift();
        var isOpen = IsShiftOpen(shiftID, out shift);
        var isEmployeeAvailable = IsEmployeeAvailable(employeeID, shift.ShiftPeriod);
        // TODO: Add check for time off
        return isOpen & isEmployeeAvailable;
    }

    public bool IsEmployeeAvailable(string employeeID, TimeRange timeRange)
    {
        DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Employees, employeeID);
        var builder = Builders<Shift>.Filter;
        var filter = builder.Eq(shift => shift.EmployeeID, employeeID) &
             builder.Lte(shift => shift.ShiftPeriod.Start, timeRange.End) &
             builder.Gte(shift => shift.ShiftPeriod.End, timeRange.Start);
        return _collectionsProvider.Shifts.CountDocuments(filter) < 1;
    }
    /// <summary>
    /// Checks if a shift is schedulable. That is it is in the future, and either has not been assigned or is open for pickup.
    /// </summary>
    /// <param name="shiftID"></param>
    /// <param name="shift"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public bool IsShiftOpen(string shiftID, out Shift shift)
    {
        shift= DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, shiftID);
        
        if (string.IsNullOrEmpty(shift.EmployeeID))
        {
            return true;
        }
        if (shift.ShiftPeriod.Start < DateTime.Now || shift.ShiftPeriod.End < DateTime.Now)
        {
            return false;
        }
        var coverageRequest = _collectionsProvider.CoverageRequests.Find(coverage => coverage.ShiftID.ToString() == shiftID).FirstOrDefault();
        if (coverageRequest == null)
        {
            return false;
        }
        return coverageRequest.CanPickup();
    }
}
using API.Models;
using MongoDB.Driver;

namespace API.Services;
public interface TimeRangeFilter
{
    DateTime start { get; set; }
    DateTime end { get; set; }
}
public interface OpenShiftQueryOptions
{
    /// <summary>
    /// Only get shifts that start within this range
    /// </summary>
    TimeRangeFilter? TimeFilter { get; set; }
}
public interface ShiftQueryOptions:OpenShiftQueryOptions
{
    /// <summary>
    /// Only get shifts assigned to this employee
    /// </summary>
    string? EmployeeIDFilter { get; set; }
}
public interface IShiftRetriever
{
    List<Shift> GetShifts(ShiftQueryOptions options);
    List<string> GetOpenShiftIDs(OpenShiftQueryOptions options);
}
public class ShiftRetriever(ICollectionsProvider provider) : IShiftRetriever
{
    ICollectionsProvider _collectionsProvider = provider;
    private FilterDefinition<Shift> BuildFilter(OpenShiftQueryOptions options,in FilterDefinitionBuilder<Shift> builder)
    {
        FilterDefinition<Shift> filter = builder.Empty;
        if (options.TimeFilter != null)
        {
            filter = filter & builder.Lt(shift => shift.Start, options.TimeFilter.start);
        }
        return filter;

    }
    private FilterDefinition<Shift> BuildFilter(ShiftQueryOptions options, in FilterDefinitionBuilder<Shift> builder)
    {
        FilterDefinition<Shift> filter = builder.Empty;
        if (options.EmployeeIDFilter != null)
        {
            filter = filter & builder.Eq(shift => shift.EmployeeID, options.EmployeeIDFilter);
        }
        filter = filter & BuildFilter((OpenShiftQueryOptions)options,builder);
        return filter;

    }
    public List<string> GetOpenShiftIDs(OpenShiftQueryOptions options)
    {

        var openToPickupShiftIDs = _collectionsProvider.CoverageRequests.Find(
            req => req.CoverageType != CoverageOptions.TradeOnly).ToList().Select(req => req.ShiftID);

        var builder = Builders<Shift>.Filter;
        var filter = options == null ? builder.Empty : BuildFilter(options, builder);
        filter = filter & builder.Where(shift => string.IsNullOrEmpty(shift.EmployeeID));
        var unassignedShifts = _collectionsProvider.Shifts.Find(filter).ToList().Select(shift => shift.Id.ToString() ?? throw new Exception("ID should never be null when reading from database."));
        var openShiftIds = openToPickupShiftIDs.Union(unassignedShifts);

        return openShiftIds.ToList();
    }

    public List<Shift> GetShifts(ShiftQueryOptions options)
    {
        return _collectionsProvider.Shifts.Find(BuildFilter(options, Builders<Shift>.Filter)).ToList();
    }
}

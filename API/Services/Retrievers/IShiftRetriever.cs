using API.Models;
using API.Models.QueryOptions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services.Retrievers;

public interface IShiftRetriever
{
    List<Shift> GetShifts(IShiftQueryOptions options);
    List<ObjectId> GetOpenShiftIDs(IShiftQueryOptions options);
}
public class ShiftRetriever(ICollectionsProvider provider) : IShiftRetriever
{
    ICollectionsProvider _collectionsProvider = provider;
    private FilterDefinition<Shift> BuildFilter(IOpenShiftQueryOptions options, in FilterDefinitionBuilder<Shift> builder)
    {
        FilterDefinition<Shift> filter = builder.Empty;
        if (options.TimeFilter != null)
        {
            filter = filter & builder.Lt(shift => shift.ShiftPeriod.Start, options.TimeFilter.Start);
        }
        return filter;

    }
    private FilterDefinition<Shift> BuildFilter(IShiftQueryOptions options, in FilterDefinitionBuilder<Shift> builder)
    {
        FilterDefinition<Shift> filter = builder.Empty;
        if (options.EmployeeIDFilter != null)
        {
            filter = filter & builder.Eq(shift => shift.EmployeeID, options.EmployeeIDFilter);
        }
        filter = filter & BuildFilter((IOpenShiftQueryOptions)options, builder);
        return filter;

    }
    public List<ObjectId> GetOpenShiftIDs(IShiftQueryOptions options)
    {

        var openToPickupShiftIDs = _collectionsProvider.CoverageRequests.Find(
            req => req.CoverageType != CoverageOptions.TradeOnly).ToList().Select(req => req.ShiftID);

        var builder = Builders<Shift>.Filter;
        var filter = options == null ? builder.Empty : BuildFilter(options, builder);
        filter = filter & builder.Where(shift => shift.EmployeeID == null);
        var unassignedShifts = _collectionsProvider.Shifts.Find(filter).ToList().Select(shift => shift.Id ?? throw new Exception("ID should never be null when reading from database."));
        var openShiftIds = openToPickupShiftIDs.Union(unassignedShifts);

        return openShiftIds.ToList();
    }

    public List<Shift> GetShifts(IShiftQueryOptions options)
    {
        return _collectionsProvider.Shifts.Find(BuildFilter(options, Builders<Shift>.Filter)).ToList();
    }
}

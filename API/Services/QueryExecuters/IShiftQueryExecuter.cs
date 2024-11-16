using API.Models;
using API.Models.QueryOptions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services.QueryExecuters;

public interface IShiftQueryExecuter
{
    List<Shift> GetShifts(ShiftQueryOptions options);
    List<ObjectId> GetOpenShiftIDs(ShiftQueryOptions options);
    List<Shift> GetOpenShifts(ShiftQueryOptions options);
}
public class ShiftQueryExecuter(ICollectionsProvider provider) : IShiftQueryExecuter
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
    public List<ObjectId> GetOpenShiftIDs(ShiftQueryOptions options)
    {

        return GetOpenShifts(options).Select(shift => shift.Id ?? throw new Exception("ID should never be null when reading from database.")).ToList();

    }
    public List<Shift> GetOpenShifts(ShiftQueryOptions options)
    {
        var openToPickupShiftIDs = _collectionsProvider.CoverageRequests
            .Find(req => req.CoverageType != CoverageOptions.TradeOnly)
            .ToList()
            .Select(req => (ObjectId?)req.ShiftID).ToList() ;
        var builder = Builders<Shift>.Filter;
        var filter = options == null ? builder.Empty : BuildFilter(options, builder);
        filter = filter & builder.Where(shift => shift.EmployeeID == null) | builder.In(shift=>shift.Id,openToPickupShiftIDs);//| builder.In(shift => shift.EmployeeID, openToPickupShiftIDs)
        var unassignedShifts = _collectionsProvider.Shifts.Find(filter).ToList();
        return unassignedShifts.ToList();
    }

    public List<Shift> GetShifts(ShiftQueryOptions options)
    {
        var filter = BuildFilter(options, Builders<Shift>.Filter);
        return _collectionsProvider.Shifts.Find(filter).ToList();
    }

}

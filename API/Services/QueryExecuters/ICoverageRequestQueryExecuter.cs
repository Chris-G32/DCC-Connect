using API.Models;
using API.Models.QueryOptions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace API.Services.QueryExecuters;

public interface ICoverageRequestQueryExecuter
{
    List<CoverageRequest> GetCoverageRequests(CoverageRequestQueryOptions options);
}
public class CoverageRequestQueryExecuter(ICollectionsProvider cp,IShiftQueryExecuter shiftQueryer) : ICoverageRequestQueryExecuter
{
    private readonly ICollectionsProvider _collectionsProvider=cp;
    private readonly IShiftQueryExecuter _shiftQueryExecuter = shiftQueryer;

    public List<CoverageRequest> GetCoverageRequests(CoverageRequestQueryOptions options)
    {
        var shifts=_shiftQueryExecuter.GetShifts(options);
        var builder = Builders<CoverageRequest>.Filter;
        var filter = builder.In(request => request.ShiftID, shifts.Select(shift => shift.Id));
        if (options.CoverageType != null)
        {
            filter = filter & builder.Eq(request => request.CoverageType, options.CoverageType);
        }
        return _collectionsProvider.CoverageRequests.Find(filter).ToList();

    }
}
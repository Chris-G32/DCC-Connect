using API.Constants;
using API.Errors;
using API.Models.QueryOptions;
using API.Models.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services.QueryExecuters;

public interface IEmployeeQueryExecuter
{
    List<User> GetEmployees(EmployeeQueryOptions options);
    User GetEmployee(ObjectId id);
}
public class EmployeeQueryExecuter(ILogger<EmployeeQueryExecuter> logger, ICollectionsProvider collectionsProvider, IAvailabiltyService availabiltyService) : IEmployeeQueryExecuter
{
    private readonly ILogger<EmployeeQueryExecuter> _logger = logger;
    private readonly ICollectionsProvider _collectionsProvider = collectionsProvider;
    private readonly IAvailabiltyService _availabiltyService = availabiltyService;
    private FilterDefinition<User> BuildFilter(IEmployeeQueryOptions options, in FilterDefinitionBuilder<User> builder)
    {
        FilterDefinition<User> filter = builder.Empty;
        if (options.EmployeeRole != null)
        {
            filter = filter & builder.Eq(employee => employee.EmployeeRole, options.EmployeeRole);
        }
        return filter;
    }
    public User GetEmployee(ObjectId id)
    {
        var result = _collectionsProvider.Users.Find(employee => employee.Id == id).FirstOrDefault() ?? throw new DCCApiException(ErrorConstants.ObjectDoesNotExistError);
        return result;
    }

    public List<User> GetEmployees(EmployeeQueryOptions options)
    {
        var builder = Builders<User>.Filter;
        var filter = BuildFilter(options, builder);
        if (options.TimeFilter != null)
        {
            return _collectionsProvider.Users.Find(filter)
                .ToEnumerable()
                .Where((employee) => { return _availabiltyService.IsKnownToExistEmployeeAvailable(employee.Id, options.TimeFilter); })
                .ToList();
        }
        return _collectionsProvider.Users.Find(filter).ToList();
    }
}
using API.Models;
using API.Models.QueryOptions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services.QueryExecuters;

public interface IEmployeeQueryExecuter
{
    List<Employee> GetEmployees(EmployeeQueryOptions options);
    ObjectId GetEmployee();
}
public class EmployeeQueryExecuter(ILogger<EmployeeQueryExecuter> logger, ICollectionsProvider collectionsProvider, IAvailabiltyService availabiltyService) : IEmployeeQueryExecuter
{
    private readonly ILogger<EmployeeQueryExecuter> _logger = logger;
    private readonly ICollectionsProvider _collectionsProvider = collectionsProvider;
    private readonly IAvailabiltyService _availabiltyService = availabiltyService;
    private FilterDefinition<Employee> BuildFilter(IEmployeeQueryOptions options, in FilterDefinitionBuilder<Employee> builder)
    {
        FilterDefinition<Employee> filter = builder.Empty;
        if (options.UniqueID != null)
        {
            filter = filter & builder.Eq(employee => employee.Id, options.UniqueID);
        }
        return filter;
    }
    public ObjectId GetEmployee()
    {
        throw new NotImplementedException();
    }

    public List<Employee> GetEmployees(EmployeeQueryOptions options)
    {
        var builder = Builders<Employee>.Filter;
        var filter = BuildFilter(options, builder);
        if (options.TimeFilter != null)
        {
            return _collectionsProvider.Employees.Find(filter)
                .ToEnumerable()
                .Where((employee) => { return _availabiltyService.IsKnownToExistEmployeeAvailable(employee.Id, options.TimeFilter); })
                .ToList();
        }
        return _collectionsProvider.Employees.Find(filter).ToList();
    }
}
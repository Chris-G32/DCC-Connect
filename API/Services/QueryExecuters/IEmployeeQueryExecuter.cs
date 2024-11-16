using API.Constants;
using API.Errors;
using API.Models;
using API.Models.QueryOptions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services.QueryExecuters;

public interface IEmployeeQueryExecuter
{
    List<Employee> GetEmployees(EmployeeQueryOptions options);
    Employee GetEmployee(ObjectId id);
}
public class EmployeeQueryExecuter(ILogger<EmployeeQueryExecuter> logger, ICollectionsProvider collectionsProvider, IAvailabiltyService availabiltyService) : IEmployeeQueryExecuter
{
    private readonly ILogger<EmployeeQueryExecuter> _logger = logger;
    private readonly ICollectionsProvider _collectionsProvider = collectionsProvider;
    private readonly IAvailabiltyService _availabiltyService = availabiltyService;
    private FilterDefinition<Employee> BuildFilter(IEmployeeQueryOptions options, in FilterDefinitionBuilder<Employee> builder)
    {
        FilterDefinition<Employee> filter = builder.Empty;
        if (options.EmployeeRole != null)
        {
            filter = filter & builder.Eq(employee => employee.EmployeeRole, options.EmployeeRole);
        }
        return filter;
    }
    public Employee GetEmployee(ObjectId id)
    {
        var result = _collectionsProvider.Employees.Find(employee => employee.Id == id).FirstOrDefault() ?? throw new DCCApiException(ErrorConstants.ObjectDoesNotExistError);
        return result;
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
using API.Constants;
using API.Models;
using MongoDB.Driver;

namespace API.Services;

public interface ICollectionsProvider
{
    public IMongoCollection<Employee> Employees { get; }
    public IMongoCollection<Shift> Shifts { get; }
    public IMongoCollection<ShiftAssignment> AssignedShifts { get; }
    public IMongoCollection<ShiftOffer> OfferedUpShifts { get; }
    public IMongoCollection<ShiftPickupRequest> TradeRequests { get; }

}
public class CollectionsProvider(IDatabaseProvider db) : ICollectionsProvider
{
    IDatabaseProvider _db = db;
    public IMongoCollection<Employee> Employees => _db.Database.GetCollection<Employee>(CollectionConstants.EmployeesCollection);

    public IMongoCollection<Shift> Shifts => _db.Database.GetCollection<Shift>(CollectionConstants.ShiftsCollection);

    public IMongoCollection<ShiftAssignment> AssignedShifts => _db.Database.GetCollection<ShiftAssignment>(CollectionConstants.AssignedShiftsCollection);

    public IMongoCollection<ShiftOffer> OfferedUpShifts => _db.Database.GetCollection<ShiftOffer>(CollectionConstants.OfferedUpShifts);

    public IMongoCollection<ShiftPickupRequest> TradeRequests => _db.Database.GetCollection<ShiftPickupRequest>(CollectionConstants.TradeRequestsCollection);
}
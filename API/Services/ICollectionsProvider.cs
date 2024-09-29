using API.Constants;
using API.Models;
using MongoDB.Driver;

namespace API.Services;

public interface ICollectionsProvider
{
    public IMongoCollection<Employee> Employees { get; }
    public IMongoCollection<Shift> Shifts { get; }
    public IMongoCollection<ShiftAssignment> AssignedShifts { get; }
}
public class CollectionsProvider(IDatabaseProvider db) : ICollectionsProvider
{
    IDatabaseProvider _db = db;
    public IMongoCollection<Employee> Employees => _db.Database.GetCollection< Employee>(CollectionConstants.Employees);

    public IMongoCollection<Shift> Shifts => _db.Database.GetCollection<Shift>(CollectionConstants.ShiftsCollection);

    public IMongoCollection<ShiftAssignment> AssignedShifts => _db.Database.GetCollection<ShiftAssignment>(CollectionConstants.AssignedShiftsCollection);
}
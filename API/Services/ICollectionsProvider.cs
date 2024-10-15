using API.Constants;
using API.Models;
using MongoDB.Driver;

namespace API.Services;

public interface ICollectionsProvider
{
    public IMongoCollection<Employee> Employees { get; }
    public IMongoCollection<Shift> Shifts { get; }
    public IMongoCollection<CoverageRequest> CoverageRequests { get; }
    public IMongoCollection<TradeOffer> TradeOffers { get; }
    public IMongoCollection<TimeOffRequest> TimeOffRequests { get; }
    public IMongoCollection<PickupOffer> PickupOffers { get; }


}
public class CollectionsProvider(IDatabaseProvider db) : ICollectionsProvider
{
    IDatabaseProvider _db = db;
    public IMongoCollection<Employee> Employees => _db.Database.GetCollection<Employee>(CollectionConstants.EmployeesCollection);

    public IMongoCollection<Shift> Shifts => _db.Database.GetCollection<Shift>(CollectionConstants.ShiftsCollection);

    public IMongoCollection<CoverageRequest> CoverageRequests => _db.Database.GetCollection<CoverageRequest>(CollectionConstants.CoverageRequestsCollection);

    public IMongoCollection<TradeOffer> TradeOffers => _db.Database.GetCollection<TradeOffer>(CollectionConstants.TradeOffersCollection);
    public IMongoCollection<PickupOffer> PickupOffers => _db.Database.GetCollection<PickupOffer>(CollectionConstants.PickupOffersCollection);

    public IMongoCollection<TimeOffRequest> TimeOffRequests => _db.Database.GetCollection<TimeOffRequest>(CollectionConstants.TimeOffRequestsCollection);

}
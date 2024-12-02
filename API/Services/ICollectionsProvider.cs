using API.Constants;
using API.Models;
using API.Models.Shifts;
using MongoDB.Driver;
using API.Models.Scheduling.Coverage;
using API.Models.Scheduling.Trading;
using API.Models.ShiftLocations;
using API.Models.Users;

namespace API.Services;

public interface ICollectionsProvider
{
    public IMongoCollection<User> Users { get; }
    //public IMongoCollection<ExternalUserInfo> Users { get; }
    public IMongoCollection<Shift> Shifts { get; }
    public IMongoCollection<CoverageRequest> CoverageRequests { get; }
    public IMongoCollection<TradeOffer> TradeOffers { get; }
    public IMongoCollection<TimeOffRequest> TimeOffRequests { get; }
    public IMongoCollection<PickupOffer> PickupOffers { get; }
    public IMongoCollection<ShiftLocation> ShiftLocations { get; }


}
public class CollectionsProvider(IDatabaseProvider db) : ICollectionsProvider
{
    IDatabaseProvider _db = db;
    public IMongoCollection<User> Users => _db.Database.GetCollection<User>(CollectionConstants.UsersCollection);

    //public IMongoCollection<ExternalUserInfo> Users => _db.Database.GetCollection<ExternalUserInfo>(CollectionConstants.UsersCollection);

    public IMongoCollection<Shift> Shifts => _db.Database.GetCollection<Shift>(CollectionConstants.ShiftsCollection);

    public IMongoCollection<CoverageRequest> CoverageRequests => _db.Database.GetCollection<CoverageRequest>(CollectionConstants.CoverageRequestsCollection);

    public IMongoCollection<TradeOffer> TradeOffers => _db.Database.GetCollection<TradeOffer>(CollectionConstants.TradeOffersCollection);
    public IMongoCollection<PickupOffer> PickupOffers => _db.Database.GetCollection<PickupOffer>(CollectionConstants.PickupOffersCollection);

    public IMongoCollection<TimeOffRequest> TimeOffRequests => _db.Database.GetCollection<TimeOffRequest>(CollectionConstants.TimeOffRequestsCollection);
    public IMongoCollection<ShiftLocation> ShiftLocations => _db.Database.GetCollection<ShiftLocation>(CollectionConstants.LocationsCollection);
}
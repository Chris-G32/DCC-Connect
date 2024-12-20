﻿namespace API.Constants;

public class CollectionConstants
{
    public static string UsersCollection { get; } = "users";
    public static string ShiftsCollection { get; } = "shifts";
    public static string CoverageRequestsCollection { get; } = "shifts.coverage.requests";
    public static string TradeOffersCollection { get; } = "shifts.coverage.trades";
    public static string PickupOffersCollection { get; } = "shifts.coverage.pickups";
    public static string TimeOffRequestsCollection { get; } = "timeoff";
    public static string LocationsCollection { get; } = "locations";
    public static string DEPRECATED { get; } = "user";
}

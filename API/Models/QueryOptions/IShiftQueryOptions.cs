﻿using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace API.Models.QueryOptions;


public interface IOpenShiftQueryOptions : IHaveTimeFilterOption;

public interface IShiftQueryOptions : IOpenShiftQueryOptions
{
    /// <summary>
    /// Only get shifts assigned to this employee
    /// </summary>
    ObjectId? EmployeeIDFilter { get; set; }
    string? RequiredRoleFilter { get; set; }
}
public class ShiftQueryOptions : IShiftQueryOptions
{
    [JsonIgnore]
    public ObjectId? EmployeeIDFilter { get; set; }
    public string? EmployeeIDFilterString { get { return EmployeeIDFilter.ToString(); } set { EmployeeIDFilter = value == null ? null : ObjectId.Parse(value); } }
    public TimeRange? TimeFilter { get; set; }
    public string? RequiredRoleFilter { get; set; }
}
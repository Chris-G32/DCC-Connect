namespace API.Models.QueryOptions;

public interface IHaveTimeFilterOption
{
    /// <summary>
    /// Takes a time range and filters the results to only include those that fall within the range. 
    /// What falling in the range means is up to the implementation of where this is used.
    /// </summary>
    TimeRange? TimeFilter { get; set; }
}

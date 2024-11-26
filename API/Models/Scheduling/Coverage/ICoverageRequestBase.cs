using System.ComponentModel.DataAnnotations;

namespace API.Models.Scheduling.Coverage;

public interface ICoverageRequestBase<ObjectIDRepresentationType>
{
    /// <summary>
    /// Start time of the off request
    /// </summary>
    public ObjectIDRepresentationType ShiftID { get; set; }
    /// <summary>
    /// Type of coverage wanted.
    /// </summary>
    public CoverageOptions CoverageType { get; set; }
    /// <summary>
    /// Optional note to leave with coverage request.
    /// </summary>
    [Length(0, 300)]
    public string? Note { get; set; }
}
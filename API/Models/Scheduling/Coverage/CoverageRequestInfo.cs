using System.ComponentModel.DataAnnotations;

namespace API.Models.Scheduling.Coverage;

public class CoverageRequestInfo : ICoverageRequestBase<string>
{
    public string ShiftID { get; set; }
    public CoverageOptions CoverageType { get; set; }
    [Length(0, 300)]
    public string? Note { get; set; }
}

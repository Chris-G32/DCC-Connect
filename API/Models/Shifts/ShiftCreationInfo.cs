using System.ComponentModel.DataAnnotations;

namespace API.Models.Shifts;

public class ShiftCreationInfo
{
    /// <summary>
    /// When the shift is taking place
    /// </summary>
    public required TimeRange ShiftPeriod { get; set; }
    /// <summary>
    /// Address of the home where the shift is taking place
    /// </summary>

    [Length(0, 70)]
    public required string LocationID { get; set; }
    /// <summary>
    /// Role of the employee working the shift
    /// </summary>
    public required string RequiredRole { get; set; }
}

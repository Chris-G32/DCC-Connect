using API.Models.Shifts;
using API.Models.Users;

namespace API.Models.Scheduling.Coverage;

public class CoverageRequestDetail
{
    public CoverageRequest CoverageRequest { get; set; }
    public Shift Shift { get; set; }
    public EmployeeInfo Employee { get; set; }
}

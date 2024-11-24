namespace API.Models.Scheduling.Trading;

public class PickupOfferCreationInfo : IPickupOfferBase<string>
{
    public string OpenShiftID { get; set; }
    public string EmployeeID { get; set; }
}

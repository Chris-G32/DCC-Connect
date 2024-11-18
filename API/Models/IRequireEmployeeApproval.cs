namespace API.Models;

public interface IRequireEmployeeApproval
{
    /// <summary>
    /// Status of manager approval. Null is no action. False denied, true approved
    /// </summary>
    bool? IsEmployeeApproved { get; set; }
}

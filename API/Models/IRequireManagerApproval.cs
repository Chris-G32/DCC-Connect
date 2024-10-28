namespace API.Models;

public interface IRequireManagerApproval
{
    /// <summary>
    /// Status of manager approval. Null is no action. False denied, true approved
    /// </summary>
    bool? IsManagerApproved { get; set; }
}

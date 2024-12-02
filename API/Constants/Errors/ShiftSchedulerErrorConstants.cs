namespace API.Constants.Errors;

public class ShiftSchedulerErrorConstants
{
    public static string EmployeeNotSchedulableError { get; } = "Employee is not schedulable for the provided shift.";
    public static string ShiftAssignmentUpdateFailedError { get; } = "Assignment/unassignment failed, perhaps the database is down or the shift/employee no longer exists..";
    public static string CannotCreateShiftInThePastError { get; } = "Cannot create a new shift in the past.";
    public static string UnassignShiftBeforeDeleteError { get; } = "Please unassign this shift before deleting it.";

}

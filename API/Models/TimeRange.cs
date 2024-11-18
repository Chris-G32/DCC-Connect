namespace API.Models;

public class TimeRange
{
    public TimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
    public TimeSpan Duration()
    {
        return End - Start;
    }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

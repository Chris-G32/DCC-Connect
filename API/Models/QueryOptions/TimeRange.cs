namespace API.Models.QueryOptions;

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

    private DateTime _end;

    public DateTime End
    {
        get { return _end; }
        set
        {
            if (value < Start)
            {
                throw new ArgumentException("End time must be after or equal to start time");
            }
            _end = value;
        }
    }
}

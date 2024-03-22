namespace GtfsReader.Structures;

public class CalendarDate
{
    public string service_id { get; }
    public DateOnly date { get; }
    public bool exception_type { get; }

    public CalendarDate(string serviceId, string date, string exceptionType)
    {
        service_id = serviceId;
        this.date = Util.Util.CreateDateFromString(date);
        exception_type = exceptionType == "1";
    }
}
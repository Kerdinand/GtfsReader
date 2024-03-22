namespace GtfsReader.Structures;

public class Calendar
{
    public string service_id { get; }
    public bool monday { get; }
    public bool tuesday { get; }
    public bool wednesday { get; }
    public bool thursday { get; }
    public bool friday { get; }
    public bool saturday { get; }
    public bool sunday { get; }
    public DateOnly start_date { get; }
    public DateOnly end_date {get;}

    public Calendar(string serviceId, string monday, string tuesday, string wednesday, string thursday, string friday, string saturday, string sunday, string startDate, string endDate)
    {
        service_id = serviceId;
        this.monday = monday == "1";
        this.tuesday = tuesday == "1";
        this.wednesday = wednesday == "1";
        this.thursday = thursday == "1";
        this.friday = friday == "1";
        this.saturday = saturday == "1";
        this.sunday = sunday== "1";
        start_date = Util.Util.CreateDateFromString(startDate);
        end_date = Util.Util.CreateDateFromString(endDate);
    }

    public bool isRunningOnDay(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Monday: return monday;
            case DayOfWeek.Tuesday: return tuesday;
            case DayOfWeek.Wednesday: return wednesday;
            case DayOfWeek.Thursday: return thursday;
            case DayOfWeek.Friday: return friday;
            case DayOfWeek.Saturday: return saturday;
            default: return sunday;
        }
    }

    public bool isRunningOnDay(DateOnly date)
    {
        if (date < start_date || date > end_date)
        {
            return false;
        }

        return isRunningOnDay(date.DayOfWeek);
    }
}
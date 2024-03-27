using GtfsReader.Util;

namespace GtfsReader.Structures;

public class Calendar
{
    public string service_id { get; set; }
    public bool monday { get; set; }
    public bool tuesday { get; set; }
    public bool wednesday { get; set; }
    public bool thursday { get; set; }
    public bool friday { get; set; }
    public bool saturday { get; set; }
    public bool sunday { get; set; }
    public DateOnly start_date { get; set; }
    public DateOnly end_date {get; set; }

    public Calendar(string[] keys, string[] values)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            switch (keys[i])
            {
                case "start_date":
                case "end_date":
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i].ToDateOnly()); break;
                case "service_id":
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i]); break;
                default:
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i] == "1"); break;
            }
        }
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
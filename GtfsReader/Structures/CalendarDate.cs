using System.Reflection;
using GtfsReader.Util;

namespace GtfsReader.Structures;

public class CalendarDate
{
    public string service_id { get; set; }
    public DateOnly date { get; set; }
    public bool exception_type { get; set; }

    public CalendarDate(string[] keys, string[] values)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            switch (keys[i])
            {
                case "date":
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i].ToDateOnly()); break;
                case "exception_type":
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i] == "1"); break;
                default:
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i]); break;
            }
        }
    }
}
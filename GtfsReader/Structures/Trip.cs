using System.Reflection;

namespace GtfsReader.Structures;

public class Trip
{
    public string route_id { get; set; }
    public string service_id { get; set; }
    public string trip_id { get; set; }
    public string trip_headsign { get; set; }
    public string trip_short_name { get; set; }
    public bool direction_id { get; set; }
    public string block_id { get; set; }
    public string shape_id { get; set; }
    public byte wheelchair_accessible { get; set; }
    public byte bikes_allowed { get; set; }

    public List<StopTime> intermediateStops;
    public Stop destination;

    public Trip(string[] keys, string[] values)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            switch (keys[i])
            {
                case "direction_id":
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i] == "1"); break;
                case "wheelchair_accessible":
                case "bikes_allowed":
                    if (values[i] == "") values[i] = "0";
                    this.GetType().GetProperty(keys[i]).SetValue(this, byte.Parse(values[i])); break;
                default:
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i]); break;
            }
        }
    }
    
    /// <summary>
    /// Add the intermediateStops List to the Trip.
    /// </summary>
    /// <param name="data">StopTime lost in chronological order.</param>
    public void AddInermediateStops(in List<StopTime> data)
    {
        intermediateStops = data;
    }
}
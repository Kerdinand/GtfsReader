using GtfsReader.Util;

namespace GtfsReader.Structures;

public class StopTime
{
    public string trip_id { get; set; }
    public TimeOnly arrival_time { get; set; }
    public TimeOnly departure_time { get; set; }
    public string stop_id { get; set;  }
    public byte stop_sequence { get; set; }
    public string stop_headsign { get; set; }
    public TimeOnly start_pickup_drop_off_window { get; set; }
    public TimeOnly end_pickup_drop_off_window { get; set; }
    public bool pickup_type { get; set; }
    public bool drop_off_type { get; set; }
    public float shape_dist_traveled { get; set; }
    public bool timepoint { get; set; }
    public string pickup_booking_rule_id { get; set; }
    public string drop_off_booking_rule_id { get; set; }
    public Stop stop { get; set; }

    public StopTime(string[] keys, string[] values)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (this.GetType().GetProperty(keys[i]) == null) continue;
            switch (keys[i])
            {
                case "arrival_time":
                case "departure_time":
                case "start_pickup_drop_off_window":
                case "end_pickup_drop_off_window":
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i].ToTimeOnly()); break;
                case "stop_sequence":
                    this.GetType().GetProperty(keys[i]).SetValue(this, byte.Parse(values[i])); break;
                default:
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i]);
            }
        }
    }
    
    public StopTime(string tripId, string arrivalTime, string departureTime, string stopId, string stopSequence, string stopHeadsign, string pickupType, string dropOffType)
    {
        trip_id = tripId;
        arrival_time = TimeOnly.Parse(Util.Util.FormatStringToTimeString(arrivalTime));
        departure_time = TimeOnly.Parse(Util.Util.FormatStringToTimeString(departureTime));
        stop_id = stopId;
        stop_sequence =  byte.Parse(stopSequence);
        stop_headsign = stopHeadsign;
        pickup_type = pickupType == "1";
        drop_off_type = dropOffType == "1";
    }

    public void SetStop(Stop stop)
    {
        this.stop = stop;
    }
}
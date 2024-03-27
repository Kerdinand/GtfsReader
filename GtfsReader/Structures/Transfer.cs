namespace GtfsReader.Structures;

public class Transfer
{
    public string from_stop_id { get; set; }
    public string to_stop_id { get; set; }
    public string from_route_id { get; set; }
    public string to_route_id { get; set; }
    public string from_trip_id { get; set; }
    public string to_trip_id { get; set; }
    public byte transfer_type { get; set; }
    public ushort min_transfer_time { get; set; }

    public Stop to_Stop;
    
    public Transfer(string[] keys, string[] values)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            switch (keys[i])
            {
                case "transfer_type":
                    this.GetType().GetProperty(keys[i]).SetValue(this, byte.Parse(values[i])); break;
                case "min_transfer_time":
                    if (values[i] == "") values[i] = "120";
                    this.GetType().GetProperty(keys[i]).SetValue(this, ushort.Parse(values[i])); break;
                default:
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i]); break;
            }
        }
    }

    public Transfer SetToStop(Stop stop)
    {
        to_Stop = stop;
        return this;
    }
}
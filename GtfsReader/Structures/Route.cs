using System.Reflection;

namespace GtfsReader.Structures;

public class Route
{
    public string route_id { get; set; }
    public string agency_id { get; set; }
    public string route_short_name { get; set; }
    public string route_long_name { get; set; }
    public string route_desc { get; set; }
    public ushort route_type { get; set; }
    public string route_url { get; set; }
    public string route_color { get; set; }
    public string route_text_color { get; set; }
    public ushort route_sort_order { get; set; }
    public byte continuous_pickup { get; set; }
    public byte continuous_drop_off { get; set; }
    public string network_id { get; set; }

    public Route(string[] keys, string[] values)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            switch (keys[i])
            {
                
                case "continuous_pickup":
                case "continuous_drop_off":
                    this.GetType().GetProperty(keys[i]).SetValue(this, byte.Parse(values[i])); break;
                case "route_sort_order":
                case "route_type":
                    this.GetType().GetProperty(keys[i]).SetValue(this, ushort.Parse(values[i])); break;
                default:
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i]); break;
            }
        }
    }
}
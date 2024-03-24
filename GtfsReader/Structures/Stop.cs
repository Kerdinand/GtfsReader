using GtfsReader.Structures.Raptor;

namespace GtfsReader.Structures;

public class Stop
{
    public string stop_id { get; set; }
    public string stop_name { get; set; }
    public string stop_code { get; set; }
    public string tts_stop_name { get; set; }
    public string stop_desc { get; set; }
    public double stop_lat { get; set; }
    public double stop_lon { get; set; }
    public string zone_id { get; set; }
    public string stop_url { get; set; }
    public byte location_type { get; set; }
    public string parent_station { get; set; }
    public string stop_timezone { get; set; }
    public byte wheelchair_boarding { get; set; }
    public sbyte level_id { get; set; }
    public string platform_code { get; set; }
    public Stop? parent_Stop { get; set; }
    public List<Stop> children_stops { get; } = new List<Stop>();

    public TimeTable timeTable { get; set; }

    public List<Transfer> transfers = new List<Transfer>();
    public List<Stop> stopsCloseBy = new List<Stop>();
    public List<int> stopsCloseByTime = new List<int>();
    
    public Label label { get; }
    
    public Stop parentStop { get; set; }

    /// <summary>
    /// Creates new Stop. Including new Label and new TimeTable.
    /// </summary>
    /// <param name="keys">Array of keys (var names)</param>
    /// <param name="values">Array of values for each attribute</param>
    public Stop(string[] keys, string[] values)
    {for (int i = 0; i < keys.Length; i++)
        {
            if (this.GetType().GetProperty(keys[i]) == null) continue;
            switch (keys[i])
            {
                case "stop_lat":
                case "stop_lon":
                    this.GetType().GetProperty(keys[i]).SetValue(this, double.Parse(values[i].Replace('.',','))); break;
                case "location_type":
                case "wheelcharair_boarding":
                    if (values[i] == "") values[i] = "100";
                    this.GetType().GetProperty(keys[i]).SetValue(this, byte.Parse(values[i])); break;
                case "level_id":
                    if (values[i] == "") values[i] = "0";
                    this.GetType().GetProperty(keys[i]).SetValue(this, sbyte.Parse(values[i])); break;
                default:
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i], null);
                    break;
            }
        }
        timeTable = new TimeTable(this);
        label = new Label();
    }

    public void ResetStopLabel()
    {
        label.Reset();
    }

    public void AddTransfer(Transfer transfer)
    {
        transfers.Add(transfer);
    }

    public override int GetHashCode()
    {
        return stop_id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{stop_id} {stop_name} {stop_lat} {stop_lon}";
    }
}
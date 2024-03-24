using GtfsReader.Structures.Raptor;

namespace GtfsReader.Structures;

public class Stop
{
    public string stop_id { get; set; }
    public string stop_name { get; set; }
    public double stop_lat { get; set; }
    public double stop_lon { get; set; }
    public string zone_id { get; set; }
    public string stop_url { get; set; }
    public byte location_type { get; set; } = 0;
    public string parent_station { get; set; } = "";
    public Stop? parent_Stop { get; set; }
    public List<Stop> children_stops { get; } = new List<Stop>();

    public TimeTable timeTable { get; set; }

    public List<Transfer> transfers = new List<Transfer>();
    public List<Stop> stopsCloseBy = new List<Stop>();
    public List<int> stopsCloseByTime = new List<int>();
    
    public Label label { get; }
    
    public Stop parentStop { get; set; }
    
    [Obsolete]
    public Stop(string stopId, string stopName, string stopLat, string stopLon, string zoneId, string stopUrl, string locationType, string parentStation)
    {
        stop_id = stopId;
        stop_name = stopName;
        stop_lat = double.Parse(stopLat) ;
        stop_lon = double.Parse(stopLon);
        zone_id = zoneId;
        stop_url = stopUrl;
        if (locationType == "") locationType = "100";
        location_type = byte.Parse(locationType);
        parent_station = parentStation;
        timeTable = new TimeTable(this);
        label = new Label();
    } 
    
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
                    if (values[i] == "") values[i] = "100";
                    this.GetType().GetProperty(keys[i]).SetValue(this, byte.Parse(values[i])); break;
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
using GtfsReader.Structures.Raptor;

namespace GtfsReader.Structures;

public class Stop
{
    public string stop_id { get; }
    public string stop_name { get; }
    public double stop_lat { get; }
    public double stop_lon { get; }
    public string zone_id { get; }
    public string stop_url { get; }
    public byte location_type { get; }
    public string parent_station { get; }
    public Stop? parent_Stop { get; set; }
    public List<Stop> children_stops { get; } = new List<Stop>();

    public TimeTable timeTable { get; set; }

    public List<Transfer> transfers = new List<Transfer>();
    public List<Stop> stopsCloseBy = new List<Stop>();
    public List<int> stopsCloseByTime = new List<int>();
    
    public Label label { get; }
    
    public Stop parentStop { get; set; }
    
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
}
namespace GtfsReader.Structures;

public class StopTime
{
    public string trip_id { get; }
    public TimeOnly arrival_time { get; }
    public TimeOnly departure_time { get; }
    public string stop_id { get; }
    public byte stop_sequence { get; }
    public string stop_headsign { get; }
    public bool pickup_type { get; }
    public bool drop_off_type { get; }
    public Stop stop { get; set; }

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
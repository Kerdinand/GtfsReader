namespace GtfsReader.Structures;

public class Trip
{
    public string route_id { get; }
    public string service_id { get; }
    public string trip_id { get; }
    public string trip_headsign { get; }
    public bool direction_id { get; }
    public string block_id { get; }
    public byte bikes_allowed { get; }

    public List<StopTime> intermediateStops;
    public Stop destination;
    public Trip(string routeId, string serviceId, string tripId, string tripHeadsign, string directionId, string blockId, string bikesAllowed)
    {
        route_id = routeId;
        service_id = serviceId;
        trip_id = tripId;
        trip_headsign = tripHeadsign;
        direction_id = directionId == "1";
        block_id = blockId;
        if (bikesAllowed == "") bikesAllowed = "0";
        bikes_allowed = byte.Parse(bikesAllowed);
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
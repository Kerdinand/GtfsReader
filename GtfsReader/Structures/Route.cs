namespace GtfsReader.Structures;

public class Route
{
    public string route_id { get; }
    public string agency_id { get; }
    public string route_short_name { get; }
    public string route_long_name { get; }
    public byte route_type { get; }
    public string route_color { get; }
    public string route_text { get; }

    public Route(string routeId, string agencyId, string routeShortName, string routeLongName, string routeType, string routeColor, string routeText)
    {
        route_id = routeId;
        agency_id = agencyId;
        route_short_name = routeShortName;
        route_long_name = routeLongName;
        if (routeType == "") routeType = "100";
        route_type = byte.Parse(routeType);
        route_color = routeColor;
        route_text = routeText;
    }
}
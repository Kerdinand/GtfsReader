using GtfsReader.Structures;
using GtfsReader.Structures.Raptor;

namespace GtfsReader;

public class Program
{
    public static void Main(string[] args)
    {
        Reader _reader = new Reader("I:\\Projekte\\GtfsReader\\GtfsReader\\Data\\");
        Raptor raptor = new Raptor(_reader.ReadStops(), _reader.ReadTrips(), _reader.ReadStopTimes(),
            _reader.ReadCalendars(), _reader.ReadCalendarDate(), _reader.ReadTransfers(), _reader.ReadRoutes());
        Dictionary<string, Stop> stops = _reader.GetStops();
        while (true)
        {
            Stop start = stops.ElementAt(new Random().Next(stops.Values.Count)).Value;
            Stop finish = stops.ElementAt(new Random().Next(stops.Count)).Value;
            //Stop start = stops["de:08212:58:1:1"];
            //Stop finish = stops["de:08235:424:1:1"];
            //List<TimeTableTrip> timeTableTrips = raptor.GetDepartingTrips(new DateTime(2024,04,17,22,00,00), 4, start);
            /*List<TimeTableTrip> timeTableTrips = raptor.GetEarliestDepartingTrips(DateTime.Now, 6, start);
            Console.WriteLine(start.stop_name);
            foreach (TimeTableTrip trip in timeTableTrips)
            {
                Console.WriteLine(
                    $"@ {trip.GetDepartureTime()} stop: {trip.intermediateStops[0].stop.stop_name}- {trip.route.route_short_name} -> {trip.trip.intermediateStops[^1].stop.stop_name}");
            }
            */
            Console.WriteLine($"Von {start.stop_name} nach {finish.stop_name}");
            Console.WriteLine(raptor.CreateResultString(raptor.GetQuickestRoute(start, DateTime.Now, 2,1,finish)));
            
            Console.ReadLine();
        }
    }
}
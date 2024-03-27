using System.Net;
using GtfsReader.Structures;
using GtfsReader.Util;

namespace GtfsReader;

public class Reader
{
    private readonly string _baseDirectory;

    private Dictionary<string, Stop> _stops = new Dictionary<string, Stop>();
    private Dictionary<string, List<StopTime>> _stopTimes = new Dictionary<string, List<StopTime>>();
    private List<Transfer> _transfers = new List<Transfer>();
    private Dictionary<string, Calendar> _calendars = new Dictionary<string, Calendar>();
    private Dictionary<string, List<CalendarDate>> _calendarDates = new Dictionary<string, List<CalendarDate>>();
    private Dictionary<string, Trip> _trips = new Dictionary<string, Trip>();
    private Dictionary<string, Agency> _agencies = new Dictionary<string, Agency>();
    private Dictionary<string, Route> _routes = new Dictionary<string, Route>();

    public Reader(string baseDirectory)
    {
        this._baseDirectory = baseDirectory;
    }
    
    public Dictionary<string, Stop> ReadStops()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "stops.txt"))
        {
            string[] keys = streamReader.ReadLine()!.ToValuesFromCsvLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                Stop newStop = new Stop(keys, line.ToValuesFromCsvLine());
                _stops[newStop.stop_id] = newStop;
                line = streamReader.ReadLine();
            }
        }
        return _stops;
    }

    public Dictionary<string, List<StopTime>> ReadStopTimes()
    {
        
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "stop_times.txt"))
        {
            string[] keys = streamReader.ReadLine()!.ToValuesFromCsvLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                StopTime newStopTime = new StopTime(keys, line.ToValuesFromCsvLine());
                if (_stopTimes.ContainsKey(newStopTime.trip_id))
                {
                    _stopTimes[newStopTime.trip_id].Add(newStopTime);
                }
                else
                {
                    _stopTimes[newStopTime.trip_id] = new List<StopTime>(new[] { newStopTime });
                }
                line = streamReader.ReadLine();
            }
        }
        return _stopTimes;
    }

    public List<Transfer> ReadTransfers()
    {
        if (!File.Exists(_baseDirectory + "transfers.txt")) return _transfers;
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "transfers.txt"))
        {
            string[] keys = streamReader.ReadLine().ToValuesFromCsvLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                Transfer newTransfer = new Transfer(keys, line.ToValuesFromCsvLine());
                _transfers.Add(newTransfer);
                line = streamReader.ReadLine();
            }
        }
        return _transfers;
    }

    public Dictionary<string, Calendar> ReadCalendars()
    {
        if (!File.Exists(_baseDirectory + "calendar.txt")) return _calendars;
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "calendar.txt"))
        {
            string[] keys = streamReader.ReadLine().ToValuesFromCsvLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                Calendar newCalendar = new Calendar(keys, line.ToValuesFromCsvLine());
                _calendars[newCalendar.service_id] = newCalendar;
                line = streamReader.ReadLine();
            }
        }
        return _calendars;
    }

    public Dictionary<string, List<CalendarDate>> ReadCalendarDate()
    {
        if (!File.Exists(_baseDirectory + "calendar_dates.txt")) return _calendarDates;
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "calendar_dates.txt"))
        {
            string[] keys = streamReader.ReadLine().ToValuesFromCsvLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                CalendarDate newCalendarDate = new CalendarDate(keys, line.ToValuesFromCsvLine());
                if (_calendarDates.ContainsKey(newCalendarDate.service_id))
                {
                    _calendarDates[newCalendarDate.service_id].Add(newCalendarDate);
                }
                else
                {
                    _calendarDates[newCalendarDate.service_id] = new List<CalendarDate>(new[] {newCalendarDate});
                }
                line = streamReader.ReadLine();
            }
        }
        return _calendarDates;
    }

    public Dictionary<string, Trip> ReadTrips()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "trips.txt"))
        {
            string[] keys = streamReader.ReadLine().ToValuesFromCsvLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                Trip newTrip = new Trip(keys, line.ToValuesFromCsvLine());
                _trips[newTrip.trip_id] = newTrip;
                line = streamReader.ReadLine();
            }
        }
        return _trips;
    }

    public Dictionary<string, Agency> ReadAgencies()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "agency.txt"))
        {
            string[] keys = streamReader.ReadLine()!.ToValuesFromCsvLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                Agency newAgency = new Agency(keys, line.ToValuesFromCsvLine());
                _agencies[newAgency.agency_id] = newAgency;
                line = streamReader.ReadLine();
            }
        }
        return _agencies;
    }

    public Dictionary<string, Route> ReadRoutes()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "routes.txt"))
        {
            string[] keys = streamReader.ReadLine().ToValuesFromCsvLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                Route newRoute = new Route(keys, line.ToValuesFromCsvLine());
                _routes[newRoute.route_id] = newRoute;
                line = streamReader.ReadLine();
            }
        }
        return _routes;
    }

    public void ReadAll()
    {
        ReadStops();
        Console.WriteLine("Stops read.");
        ReadStopTimes();
        Console.WriteLine("StopTimes read");
        ReadTransfers();
        Console.WriteLine("Transfers read");
        ReadCalendars();
        Console.WriteLine("Calendars read");
        ReadCalendarDate();
        Console.WriteLine("CalendarDates read");
        ReadTrips();
        Console.WriteLine("Trips read");
        ReadAgencies();
        Console.WriteLine("Agencies read");
        ReadRoutes();
        Console.WriteLine("Routes read");
    }
}
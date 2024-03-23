using GtfsReader.Structures;

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
            streamReader.ReadLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                string[] c = Util.Util.GetValuesFromCsvLine(line);
                _stops[c[0]] = new Stop(c[0], c[1], c[2].Replace('.',','), c[3].Replace('.',','), c[4], c[5], c[6], c[7]);
                line = streamReader.ReadLine();
            }
        }

        return _stops;
    }

    public Dictionary<string, List<StopTime>> ReadStopTimes()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "stop_times.txt"))
        {
            streamReader.ReadLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                string[] c = Util.Util.GetValuesFromCsvLine(line);
                StopTime s = new StopTime(c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7]);
                if (_stopTimes.ContainsKey(c[0]))
                {
                    _stopTimes[c[0]].Add(s);
                }
                else
                {
                    List<StopTime> tmp = new List<StopTime>();
                    tmp.Add(s);
                    _stopTimes[c[0]] = tmp;
                }

                line = streamReader.ReadLine();
            }
        }
        return _stopTimes;
    }

    public List<Transfer> ReadTransfers()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "transfers.txt"))
        {
            streamReader.ReadLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                string[] c = Util.Util.GetValuesFromCsvLine(line);
                _transfers.Add(new Transfer(c[0],c[1],c[2],c[3]));
                line = streamReader.ReadLine();
            }
        }
        return _transfers;
    }

    public Dictionary<string, Calendar> ReadCalendars()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "calendar.txt"))
        {
            streamReader.ReadLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                string[] c = Util.Util.GetValuesFromCsvLine(line);
                _calendars[c[0]] = new Calendar(c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7], c[8], c[9]);
                line = streamReader.ReadLine();
            }
        }
        return _calendars;
    }

    public Dictionary<string, List<CalendarDate>> ReadCalendarDate()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "calendar_dates.txt"))
        {
            streamReader.ReadLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                string[] c = Util.Util.GetValuesFromCsvLine(line);
                CalendarDate cd = new CalendarDate(c[0], c[1], c[2]);
                if (_calendarDates.ContainsKey(c[0]))
                {
                    _calendarDates[c[0]].Add(cd);
                }
                else
                {
                    List<CalendarDate> tmp = new List<CalendarDate>();
                    tmp.Add(cd);
                    _calendarDates[c[0]] = tmp;
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
            streamReader.ReadLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                string[] c = Util.Util.GetValuesFromCsvLine(line);
                _trips[c[2]] = new Trip(c[0], c[1], c[2], c[3], c[4], c[5], c[6]);
                line = streamReader.ReadLine();
            }
        }
        return _trips;
    }

    public Dictionary<string, Agency> ReadAgencies()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "agency.txt"))
        {
            streamReader.ReadLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                string[] c = Util.Util.GetValuesFromCsvLine(line);
                _agencies[c[0]] = new Agency(c[0], c[1], c[2], c[3], c[4], c[5]);
                line = streamReader.ReadLine();
            }
        }
        return _agencies;
    }

    public Dictionary<string, Route> ReadRoutes()
    {
        using (StreamReader streamReader = new StreamReader(_baseDirectory + "routes.txt"))
        {
            streamReader.ReadLine();
            string? line = streamReader.ReadLine();
            while (line != null)
            {
                string[] c = Util.Util.GetValuesFromCsvLine(line);
                _routes[c[0]] = new Route(c[0], c[1], c[2], c[3], c[4], c[5], c[6]);
                line = streamReader.ReadLine();
            }
        }
        return _routes;
    }
}
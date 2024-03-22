using System.Xml;
using GtfsReader.Structures;
using GtfsReader.Structures.Raptor;

namespace GtfsReader;

public class Raptor
{
    /*
     *  Contains all the raw read Data. 
     *  Is set by constructor.
     */
    
    private Dictionary<string, Stop> _stops;
    private Dictionary<string, Trip> _trips;
    private Dictionary<string, List<StopTime>> _stopTimeLists;
    private Dictionary<string, Calendar> _calendars;
    private Dictionary<string, List<CalendarDate>> _calendarDates;
    private List<Transfer> _transfers;
    private Dictionary<string, Route> _routes;

    private Dictionary<string, TimeTable> _timeTables = new Dictionary<string, TimeTable>();
    public Raptor(Dictionary<string, Stop> stops, Dictionary<string, Trip> trips, Dictionary<string, List<StopTime>> stopTimeLists, Dictionary<string, Calendar> calendars, Dictionary<string, List<CalendarDate>> calendarDates, List<Transfer> transfers, Dictionary<string, Route> routes)
    {
        _stops = stops;
        _trips = trips;
        _stopTimeLists = stopTimeLists;
        _calendars = calendars;
        _calendarDates = calendarDates;
        _transfers = transfers;
        _routes = routes;
        SetIntermediateStops();
        AddTransferStops();
        CreateTimeTables();
        SetParentStops();
        SetNearbyStops();
        SetStopsToStopTimes();
    }

    private void SetStopsToStopTimes()
    {
        foreach (List<StopTime> stopTimes in _stopTimeLists.Values)
        {
            foreach (StopTime stopTime in stopTimes)
            {
                stopTime.SetStop(_stops[stopTime.stop_id]);
            }
        }
    }
    
    private void SetNearbyStops()
    {
        foreach (Stop stop in _stops.Values)
        {
            foreach (Stop s in _stops.Values)
            {
                if (s.Equals(stop)) continue;
                if (s.parentStop == stop.parentStop) continue;
                double time;
                if ((time = Util.Util.CalculateDistance(stop, s)) < 0.6)
                {
                    stop.stopsCloseBy.Add(s);
                    stop.stopsCloseByTime.Add((int) (time * 17)+1);
                }
            }
        }
    }
    
    // Add intermediateStops to the corresponding trips
    private void SetIntermediateStops()
    {
        foreach (List<StopTime> data in _stopTimeLists.Values)
        {
            _trips[data[0].trip_id].AddInermediateStops(data);
        }
    }
    
    // Sets all the Transfers of each Stops
    private void AddTransferStops()
    {
        foreach (Transfer transfer in _transfers)
        {
            transfer.SetToStop(_stops[transfer.to_Stop_id]);
            _stops[transfer.from_stop_id].AddTransfer(transfer);
        }
    }

    // Creates the TimeTables, including all the timetable trips, for each station.
    private void CreateTimeTables()
    {
        foreach (Trip trip in _trips.Values)
        {
            StopTime[] tmp = new List<StopTime>(trip.intermediateStops).ToArray();
            for (int i = 0; i + 1 < tmp.Length; i++)
            {
                TimeTableTrip tr = new TimeTableTrip(tmp.Skip(i+1).ToList(), _routes[trip.route_id], trip);
                tr.SetDepartureTime(tmp[i].departure_time);
                _stops[tmp[i].stop_id].timeTable.AddTimeTableTrip(tr);
            }
        }
    }

    public Stop GetStop(string id)
    {
        return _stops[id];
    }

    
    
    public List<TimeTableTrip> GetDepartingTrips(DateTime currentTime, int maximumWaitingTime, Stop stopMain)
    {
        HashSet<TimeTableTrip> runningTrips = new HashSet<TimeTableTrip>();
        HashSet<string> departures = new HashSet<string>();
        // Add all stops if stop is a parent stop and contains children
        if (stopMain.parent_station == "" && stopMain.children_stops.Count != 0)
        {
            foreach (Stop stop in stopMain.children_stops)
            {
                foreach (TimeTableTrip timeTableTrip in stop.timeTable.GetDepartingTrips(currentTime, maximumWaitingTime))
                {
                    if (departures.Contains(timeTableTrip.GetIdentString(_stops))) continue;
                    runningTrips.Add(timeTableTrip);
                    departures.Add(timeTableTrip.GetIdentString(_stops));
                }
            }
        }
        // Add all stops if stop is NOT a parent station, but contains parent stop
        if (stopMain.parent_station != "" && stopMain.children_stops.Count == 0)
        {
            foreach (Stop stop in stopMain.parentStop.children_stops)
            {
                foreach (TimeTableTrip timeTableTrip in stop.timeTable.GetDepartingTrips(currentTime, maximumWaitingTime))
                {
                    if (departures.Contains(timeTableTrip.GetIdentString(_stops))) continue;
                    runningTrips.Add(timeTableTrip);
                    departures.Add(timeTableTrip.GetIdentString(_stops));
                }
            }
        }
        // Add all stops if stop is not a parent and also not contains children
        if (stopMain.parent_station == "" && stopMain.children_stops.Count == 0)
        {
            foreach (TimeTableTrip timeTableTrip in stopMain.timeTable.GetDepartingTrips(currentTime, maximumWaitingTime))
            {
                if (departures.Contains(timeTableTrip.GetIdentString(_stops))) continue;
                runningTrips.Add(timeTableTrip);
                departures.Add(timeTableTrip.GetIdentString(_stops));
            }
        }
        
        List<TimeTableTrip> result = runningTrips.ToList();
        
        result.Sort((TimeTableTrip a, TimeTableTrip b) => a.intermediateStops[0].arrival_time.CompareTo(b.intermediateStops[0].arrival_time));

        return result;
    }

    private List<Stop> GetStopsInVicinity(Stop stop, double distance)
    {
        List<Stop> result = new List<Stop>();
        foreach (Stop s in _stops.Values)
        {
            if (s.parentStop == stop.parentStop) continue;
            if (Util.Util.CalculateDistance(s, stop) < distance)
            {
                result.Add(s);
            }
        }

        return result;
    }

    private void SetParentStops()
    {
        foreach (Stop stop in _stops.Values)
        {
            if (stop.parent_station != "")
            {
                stop.parentStop = _stops[stop.parent_station];
                stop.parentStop.children_stops.Add(stop);
            }
            else
            {
                stop.parentStop = stop;
            }
        }
    }

    private bool isEarlierTime(DateTime labelTime, TimeOnly stopTime, DateTime originalTime)
    {
        DateTime newTime = new DateTime(originalTime.Year, originalTime.Month, originalTime.Day, stopTime.Hour,
            stopTime.Minute, stopTime.Second);
        if (stopTime < TimeOnly.FromDateTime(originalTime))
        {
            //Console.WriteLine(stopTime + "  -  " + TimeOnly.FromDateTime(originalTime));
            newTime = newTime.AddDays(1);
        }

        return labelTime > newTime;
    }
    
    public List<Stop> GetQuickestRoute(Stop start, DateTime startTime, int numberOfRounds, int maximumWaitingTime, Stop end)
    {
        DateTime originalStartTime = startTime;
        foreach (Stop stop in _stops.Values)
        {
            stop.ResetStopLabel();
        }

        start = start.parentStop;
        end = end.parentStop;
        HashSet<Stop> markedStops = new HashSet<Stop>();
        markedStops.Add(start);
        start.label.arrivalTime = startTime;

        for (int i = 0; i < start.stopsCloseBy.Count; i++)
        {
            Stop currentStop = start.stopsCloseBy[i].parentStop;
            if (currentStop.stop_id == start.stop_id) continue;
            currentStop.label.origin = start;
            currentStop.label.exitTrip = null;
            currentStop.label.arrivalTime = startTime.AddMinutes(start.stopsCloseByTime[i]+1);
            start.label.departureTime = currentStop.label.arrivalTime.AddMinutes(-(start.stopsCloseByTime[i]+1));
            markedStops.Add(currentStop);
        }
        

        for (int index = 0; index < numberOfRounds; index++)
        {
            HashSet<Stop> updatedStops = new HashSet<Stop>();
            foreach (Stop stop in markedStops)
            {
                List<TimeTableTrip> departingTrips =
                    GetDepartingTrips(stop.label.arrivalTime, maximumWaitingTime, stop);
                foreach (TimeTableTrip trip in departingTrips)
                {
                    foreach (StopTime stopTime in trip.intermediateStops)
                    {
                        Stop currentStop = _stops[stopTime.stop_id].parentStop;
                        if (currentStop.stop_id == start.stop_id) continue;
                        if (!isEarlierTime(currentStop.label.arrivalTime, stopTime.arrival_time, originalStartTime)) continue;
                        currentStop.label.arrivalTime = new DateTime(startTime.Year, startTime.Month, startTime.Day,
                            stopTime.arrival_time.Hour, stopTime.arrival_time.Minute, stopTime.arrival_time.Second);
                        if (currentStop.label.arrivalTime < originalStartTime)
                        {
                            currentStop.label.arrivalTime = currentStop.label.arrivalTime.AddDays(1);
                        }
                        currentStop.label.origin = stop;
                        currentStop.label.exitTrip = trip;
                        TimeOnly t = trip.GetDepartureTime();
                        stop.label.departureTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, t.Hour,
                            t.Minute, t.Second);
                        stop.label.departingTrip = trip;
                        if (stop.label.departureTime < stop.label.arrivalTime)
                        {
                            stop.label.departureTime = stop.label.departureTime.AddDays(1);
                        }
                        updatedStops.Add(currentStop);
                    }
                }
            }
            
            // Prüfe definierte Transfers 
            foreach (Stop stop in markedStops)
            {
                foreach (Transfer transfer in stop.transfers)
                {
                    Stop currentStop = transfer.to_Stop.parentStop;
                    if (currentStop.stop_id == start.stop_id) continue;
                    if (currentStop.label.arrivalTime < stop.label.arrivalTime.AddSeconds(transfer.min_transfer_time)) continue;
                    currentStop.label.isTransfer = true;
                    currentStop.label.origin = stop;
                    currentStop.label.exitTrip = null;
                    stop.label.departingTrip = null;
                    currentStop.label.arrivalTime = stop.label.arrivalTime.AddSeconds(transfer.min_transfer_time);
                    stop.label.departureTime = stop.label.arrivalTime;
                    updatedStops.Add(stop);
                }
            }

            // Prüfe alle Stationen in Laufweite
            foreach (Stop stop in markedStops)
            {
                for (int i = 0; i < stop.stopsCloseBy.Count; i++)
                {
                    Stop currentStop = stop.stopsCloseBy[i].parentStop;
                    if (currentStop.stop_id == start.stop_id) continue;
                    if (currentStop.label.arrivalTime < stop.label.arrivalTime.AddMinutes(stop.stopsCloseByTime[i])) continue;
                    currentStop.label.origin = stop;
                    currentStop.label.exitTrip = null;
                    currentStop.label.arrivalTime = stop.label.arrivalTime.AddMinutes(stop.stopsCloseByTime[i]);
                    stop.label.departureTime = stop.label.arrivalTime;
                    stop.label.departingTrip = null;
                    updatedStops.Add(stop);
                }
            }
            markedStops = new HashSet<Stop>(updatedStops);
            updatedStops.Clear();
        }

        List<Stop> stopsNearEnd = GetStopsInVicinity(end.parentStop, 0.3);

        foreach (Stop s in stopsNearEnd)
        {
            if (s.label.arrivalTime == DateTime.MaxValue) continue;
            if (end.label.arrivalTime > s.label.arrivalTime.AddMinutes(Util.Util.CalculateDistance(end, s) * 10))
            {
                end.label.arrivalTime = s.label.arrivalTime.AddMinutes(Util.Util.CalculateDistance(end, s) * 10);
                end.label.origin = s;
                end.label.exitTrip = null;
            }
        }
        
        List<Stop> result = new List<Stop>();

        if (end.label.origin == null) return result;
        int counter = 0;
        while (end != null && counter < 99)
        {
            result.Add(end);
            if (end.parentStop.stop_id == start.parentStop.stop_id) break;
            end = end.label.origin;
            counter++;
        }
        
        result.Reverse();

        return result;
    }
    
    
    public List<Stop> GetQuickestRoute(string start, string end, int numberOfRounds, int maximumWaitingTime, DateTime startTime)
    {
        return GetQuickestRoute(_stops[start], startTime, numberOfRounds, maximumWaitingTime, _stops[end]);
    }

    public string CreateResultString(List<Stop> result)
    {
        string tripInfo = "";
        tripInfo += result[0].label.departureTime + "- \n";
        for (int i = 0; i + 1 < result.Count; i++)
        {
            Stop stop = result[i];
            Stop nextStop = result[i + 1];
            string line = "Zu Fuß";
            string depTime = stop.label.departureTime.ToString();
            if (nextStop.label.isTransfer)
            {
                line = "Transfer";
            }
            if (nextStop.label.exitTrip != null)
            {
                string headSign = _stops[(nextStop.label.exitTrip.intermediateStops[^1].stop_id)].stop_name;
                if (nextStop.label.exitTrip.trip.trip_headsign != "")
                {
                    headSign = nextStop.label.exitTrip.trip.trip_headsign;
                }

                line = nextStop.label.exitTrip.route.route_short_name + " nach: " + headSign;
                depTime = nextStop.label.exitTrip.GetDepartureTime().ToString();
            }

            tripInfo += depTime + " - " + stop.stop_name + " -> " + nextStop.stop_name + " @" +
                        nextStop.label.arrivalTime + " using: " + line + "\n";
        }

        return tripInfo;
    }
}
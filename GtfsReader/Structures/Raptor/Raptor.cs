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
        SetNearbyStops();
        Console.WriteLine("Data for raptor is set");
        SetIntermediateStops();
        Console.WriteLine("Intermediate Stops set");
        AddTransferStops();
        Console.WriteLine("Add transfers set");
        CreateTimeTables();
        Console.WriteLine("Timetables set");
        SetParentStops();
        Console.WriteLine("Parent stops set");
        SortAllTimeTableTrips();
        Console.WriteLine("Timetable sorted");
        SetStopsToStopTimes();
        Console.WriteLine("Nearby stops set");
        Console.WriteLine("finished");
    }

    private void SortAllTimeTableTrips()
    {
        foreach (Stop stop in _stops.Values)
        {
            stop.timeTable.SortTimeTableData();
        }
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
        /*
        for (int i = 0; i < _stops.Count; i++)
        {
            if (i % 1000 == 0) Console.WriteLine("Check " + i);
            for (int j = i + 1; j < _stops.Count; j++)
            {
                double time;
                Stop stop1 = _stops.ElementAt(i).Value;
                Stop stop2 = _stops.ElementAt(j).Value;
                if ((time = Util.Util.CalculateDistance(stop1, stop2)) < 0.4)
                {
                    stop1.stopsCloseBy.Add(stop2);
                    stop1.stopsCloseByTime.Add((int) (time * 17)+1);
                    stop2.stopsCloseBy.Add(stop1);
                    stop2.stopsCloseByTime.Add((int) (time * 17)+1);
                }
            }
        }*/
        int counter = 0;
        foreach (Stop stop in _stops.Values)
        {
            if (counter % 1000 == 0) Console.WriteLine(((float)counter /_stops.Count));
            counter++;
            foreach (Stop s in _stops.Values)
            {
                if (s.Equals(stop)) continue;
                if (s.parentStop == stop.parentStop) continue;
                double time;
                if ((time = Util.Util.CalculateDistance(stop, s)) < 0.25)
                {
                    stop.stopsCloseBy.Add(s);
                    stop.stopsCloseByTime.Add((int) (time * 15));
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
            transfer.SetToStop(_stops[transfer.to_stop_id]);
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

    
    
    public List<TimeTableTrip> GetDepartingTrips(DateTime currentTime, int maximumWaitingTime, Stop stopMain, bool onlySelf = false)
    {
        HashSet<TimeTableTrip> runningTrips = new HashSet<TimeTableTrip>();
        HashSet<string> departures = new HashSet<string>();

        if (onlySelf)
        {
            foreach (TimeTableTrip timeTableTrip in stopMain.timeTable.GetDepartingTrips(currentTime, maximumWaitingTime, _calendars, _calendarDates))
            {
                if (departures.Contains(timeTableTrip.GetIdentString(_stops))) continue;
                runningTrips.Add(timeTableTrip);
                departures.Add(timeTableTrip.GetIdentString(_stops));
            }
            return runningTrips.ToList();
        }
        
        // Add all stops if stop is a parent stop and contains children
        if (stopMain.parent_station == "" && stopMain.children_stops.Count != 0)
        {
            foreach (Stop stop in stopMain.children_stops)
            {
                foreach (TimeTableTrip timeTableTrip in stop.timeTable.GetDepartingTrips(currentTime, maximumWaitingTime, _calendars, _calendarDates))
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
                foreach (TimeTableTrip timeTableTrip in stop.timeTable.GetDepartingTrips(currentTime, maximumWaitingTime, _calendars, _calendarDates))
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
            foreach (TimeTableTrip timeTableTrip in stopMain.timeTable.GetDepartingTrips(currentTime, maximumWaitingTime, _calendars, _calendarDates))
            {
                if (departures.Contains(timeTableTrip.GetIdentString(_stops))) continue;
                runningTrips.Add(timeTableTrip);
                departures.Add(timeTableTrip.GetIdentString(_stops));
            }
        }

        List<TimeTableTrip> tmp = runningTrips.ToList();
        tmp.Sort((TimeTableTrip a, TimeTableTrip b) => a.GetDepartureTime().CompareTo(b.GetDepartureTime()));
        if (tmp.Count == 0) return tmp;
        if (tmp[0].GetDepartureTime() < TimeOnly.FromDateTime(currentTime))
        {
            List<TimeTableTrip> nextDayDep = new List<TimeTableTrip>();
            List<TimeTableTrip> thisDayDep = new List<TimeTableTrip>();
            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i].GetDepartureTime() > TimeOnly.FromDateTime(currentTime))
                {
                    thisDayDep.Add(tmp[i]);
                }
                else
                {
                    nextDayDep.Add(tmp[i]);
                }
            }
            tmp = thisDayDep.Concat(nextDayDep).ToList();
        }
        return tmp;
    }

    public List<TimeTableTrip> GetEarliestDepartingTrips(DateTime currentTime, int maximumWaitingTime, Stop stopMain, bool onlySelf = false)
    {
        List<TimeTableTrip> allDepartingTrips = GetDepartingTrips(currentTime, maximumWaitingTime, stopMain, onlySelf);
        HashSet<string> filterSet = new HashSet<string>();
        List<TimeTableTrip> result = new List<TimeTableTrip>();
        foreach (TimeTableTrip trip in allDepartingTrips)
        {
            if (!filterSet.Contains(trip.GetIdentStringSimple()))
            {
                result.Add(trip);
                filterSet.Add(trip.GetIdentStringSimple());
            }
        }
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

        return labelTime >= newTime;
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

        if (GetEarliestDepartingTrips(start.label.arrivalTime, maximumWaitingTime, start, false).Count == 0)
        {
            List<Stop> stopsNearStart = GetStopsInVicinity(start, 1);
            foreach (Stop stop in stopsNearStart)
            {
                stop.label.arrivalTime = start.label.arrivalTime.AddMinutes(Util.Util.CalculateDistance(stop, start) * 20);
                stop.label.origin = start;
                stop.label.exitTrip = null;
                markedStops.Add(stop);
            }
        }
        

        for (int index = 0; index < numberOfRounds; index++)
        {
            HashSet<Stop> updatedStops = new HashSet<Stop>();
            foreach (Stop stop in markedStops)
            {
                List<TimeTableTrip> departingTrips =
                    GetEarliestDepartingTrips(stop.label.arrivalTime, maximumWaitingTime, stop, false);
                foreach (TimeTableTrip trip in departingTrips)
                {
                    foreach (StopTime stopTime in trip.intermediateStops)
                    {
                        Stop currentStop = _stops[stopTime.stop_id].parentStop;
                        if (currentStop.stop_id == start.stop_id) continue;
                        if (!isEarlierTime(currentStop.label.arrivalTime, stopTime.arrival_time.AddMinutes(1), originalStartTime)) continue;
                        if (currentStop.label.exitTrip != null && currentStop.label.exitTrip.GetIdentStringSimple() == trip.GetIdentStringSimple()) continue;
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
        
        List<Stop> stopsNearEnd = GetStopsInVicinity(end.parentStop, 1);

        foreach (Stop s in stopsNearEnd)
        {
            if (s.label.arrivalTime == DateTime.MaxValue) continue;
            if (end.label.arrivalTime > s.label.arrivalTime.AddMinutes(Util.Util.CalculateDistance(end, s) * 20))
            {
                end.label.arrivalTime = s.label.arrivalTime.AddMinutes(Util.Util.CalculateDistance(end, s) * 20);
                end.label.origin = s;
                end.label.exitTrip = null;
            }
        }
        
        List<Stop> result = new List<Stop>();

        if (end.label.origin == null) return result;
        int counter = 0;
        while (end != null && counter < numberOfRounds*2)
        {
            result.Add(end);
            if (end.parentStop.stop_id == start.parentStop.stop_id) break;
            end = end.label.origin;
            counter++;
        }
        
        result.Reverse();

        if (result.Count > 2 && result[1].label.exitTrip != null)
        {
            SetLatestFittingDirectTrip(result[0], result[1], result[0].label.arrivalTime,
                result[1].label.departingTrip.GetDepartureTime(), maximumWaitingTime);
        }
        return result;
    }

    public TimeTableTrip SetLatestFittingDirectTrip(Stop start, Stop end,DateTime departureTime, TimeOnly arrivalTime, int maximumWaitingTime)
    {
        List<TimeTableTrip> departingTripsToCheck = GetDepartingTrips(departureTime, maximumWaitingTime, start);
        TimeTableTrip bestFit = null;
        foreach (TimeTableTrip trip in departingTripsToCheck)
        {
            foreach (StopTime stopTime in trip.intermediateStops)
            {
                if (stopTime.stop.parentStop.stop_id == end.parentStop.stop_id &&
                    arrivalTime >= stopTime.arrival_time)
                {
                    TimeOnly depTime = trip.GetDepartureTime();
                    start.parentStop.label.departureTime = new DateTime(departureTime.Year, departureTime.Month,
                        departureTime.Day, depTime.Hour, depTime.Minute, depTime.Second);
                    end.label.exitTrip = trip;
                    depTime = stopTime.arrival_time;
                    end.label.arrivalTime = new DateTime(departureTime.Year, departureTime.Month,
                        departureTime.Day, depTime.Hour, depTime.Minute, depTime.Second);
                    bestFit = trip;
                }
            }
        }
        return bestFit;
    }
    
    public List<Stop> GetQuickestRoute(string start, string end, int numberOfRounds, int maximumWaitingTime, DateTime startTime)
    {
        return GetQuickestRoute(_stops[start], startTime, numberOfRounds, maximumWaitingTime, _stops[end]);
    }

    public string CreateResultString(List<Stop> result)
    {
        if (result.Count == 0) return "no way";
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

                line = nextStop.label.exitTrip.route.route_short_name + " nach: " + headSign + " " + nextStop.label.exitTrip.trip.trip_id;
                depTime = nextStop.label.exitTrip.GetDepartureTime().ToString();
            }

            tripInfo += depTime + " - " + stop.stop_name + " -> " + nextStop.stop_name + " @" +
                        nextStop.label.arrivalTime + " using: " + line + "\n";
        }

        return tripInfo;
    }
}
namespace GtfsReader.Structures.Raptor;

public class TimeTableTrip
{
    public Route route { get; }
    public List<StopTime> intermediateStops;
    public Trip trip;
    private TimeOnly departureTime;

    public string GetIdentString(Dictionary<string, Stop> stops)
    {
        return route.route_short_name + intermediateStops[0].departure_time +
               stops[intermediateStops[^1].stop_id].stop_name;
    }
    public TimeTableTrip(List<StopTime> intermediateStops, Route route, Trip trip)
    {
        this.intermediateStops = intermediateStops;
        this.route = route;
        this.trip = trip;
        
    }

    public void SetDepartureTime(TimeOnly time)
    {
        departureTime = time;
    }

    public TimeOnly GetDepartureTime()
    {
        return departureTime;
    }

    public bool TripIsRunning(DateOnly date, Dictionary<string, Calendar> calendars, Dictionary<string, List<CalendarDate>> calendarDates)
    {
        if (calendarDates.ContainsKey(trip.service_id))
        {
            foreach (CalendarDate calendarDate in calendarDates[trip.service_id])
            {
                if (calendarDate.date == date) return calendarDate.exception_type;
            }
        }
        return calendars[this.trip.service_id].isRunningOnDay(date);
    }

    public int CompareTo(TimeTableTrip other)
    {
        return this.intermediateStops[0].departure_time.CompareTo(other.intermediateStops[0].departure_time);
    }
}
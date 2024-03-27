namespace GtfsReader.Structures.Raptor;

public class TimeTable
{
    private Stop stop;
    public List<TimeTableTrip> timeTableData = new List<TimeTableTrip>();

    
    public TimeTable(Stop stop)
    {
        this.stop = stop;
    }

    public void AddTimeTableTrip(TimeTableTrip trip)
    {
        timeTableData.Add(trip);
    }

    public void SortTimeTableData()
    {
        timeTableData.Sort((TimeTableTrip a, TimeTableTrip b) => a.CompareTo(b));
    }

    public List<TimeTableTrip> GetDepartingTrips(DateTime internalTime ,int maximumWeightingTime, Dictionary<string, Calendar> calendars, Dictionary<string, List<CalendarDate>> calendarDates)
    {
        DateTime currentTime = internalTime;
        TimeOnly time = TimeOnly.FromDateTime(currentTime);
        List<TimeTableTrip> tripsInTimeFrame = new List<TimeTableTrip>();
        if (time > time.AddHours(maximumWeightingTime))
        {
            for (int i = 0; i < timeTableData.Count; i++)
            {
                if (timeTableData[i].GetDepartureTime() < time) continue;
                if (!timeTableData[i].TripIsRunning(DateOnly.FromDateTime(currentTime), calendars, calendarDates)) continue;
                tripsInTimeFrame.Add(timeTableData[i]);
            }
            currentTime = currentTime.AddDays(1);
            for (int i = 0; i < timeTableData.Count; i++)
            {
                if (timeTableData[i].GetDepartureTime() > time.AddHours(maximumWeightingTime)) break;
                if (!timeTableData[i].TripIsRunning(DateOnly.FromDateTime(currentTime), calendars, calendarDates)) continue;
                tripsInTimeFrame.Add(timeTableData[i]);
            }
        }
        else
        {
            for (int i = 0; i < timeTableData.Count; i++)
            {
                if (timeTableData[i].GetDepartureTime() < time) continue;
                if (timeTableData[i].GetDepartureTime() > time.AddHours(maximumWeightingTime)) continue;
                if (!timeTableData[i].TripIsRunning(DateOnly.FromDateTime(currentTime), calendars, calendarDates)) continue;
                tripsInTimeFrame.Add(timeTableData[i]);
            }
        }
        return tripsInTimeFrame;
    }
}
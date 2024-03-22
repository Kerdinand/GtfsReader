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

    public List<TimeTableTrip> GetDepartingTrips(DateTime currentTime ,int maximumWeightingTime)
    {
        TimeOnly time = TimeOnly.FromDateTime(currentTime);
        List<TimeTableTrip> tripsInTimeFrame = new List<TimeTableTrip>();
        if (time > time.AddHours(maximumWeightingTime))
        {
            for (int i = 0; i < timeTableData.Count; i++)
            {
                if (timeTableData[i].GetDepartureTime() < time) continue;
                tripsInTimeFrame.Add(timeTableData[i]);
            }

            for (int i = 0; i < timeTableData.Count; i++)
            {
                if (timeTableData[i].GetDepartureTime() > time.AddHours(maximumWeightingTime)) break;
                tripsInTimeFrame.Add(timeTableData[i]);
            }
        }
        else
        {
            for (int i = 0; i < timeTableData.Count; i++)
            {
                if (timeTableData[i].GetDepartureTime() < time) continue;
                if (timeTableData[i].GetDepartureTime() > time.AddHours(maximumWeightingTime)) continue;
                tripsInTimeFrame.Add(timeTableData[i]);
            }
        }
        return tripsInTimeFrame;
    }
}
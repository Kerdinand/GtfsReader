namespace GtfsReader.Structures.Raptor;

public class Label
{
    public DateTime arrivalTime = DateTime.MaxValue;
    public DateTime departureTime = DateTime.MaxValue;
    public Stop origin = null;
    public TimeTableTrip? exitTrip = null;
    public TimeTableTrip? departingTrip = null;
    public bool isTransfer = false;

    public void Reset()
    {
        arrivalTime = DateTime.MaxValue;
        departureTime = DateTime.MaxValue;
        origin = null;
        exitTrip = null;
        departingTrip = null;
        isTransfer = false;
    }
}
namespace GtfsReader.Structures;

public class Transfer
{
    public string from_stop_id { get; }
    public string to_Stop_id { get; }
    public byte transfer_type { get; }
    public ushort min_transfer_time { get; }

    public Stop to_Stop;

    public Transfer(string fromStopId, string toStopId, string transferType, string minTransferTime)
    {
        from_stop_id = fromStopId;
        to_Stop_id = toStopId;
        transfer_type = byte.Parse(transferType);
        min_transfer_time = ushort.Parse(minTransferTime);
    }

    public Transfer SetToStop(Stop stop)
    {
        to_Stop = stop;
        return this;
    }
}
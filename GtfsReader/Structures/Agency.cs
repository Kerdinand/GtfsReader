namespace GtfsReader.Structures;

public class Agency
{
    public string agency_id { get; set; }
    public string agency_name { get; set; }
    public string agency_url { get; set; }
    public string agency_timezone { get; set; }
    public string agency_lang { get; set; }
    public string agency_phone { get; set; }
    public string agency_fare_url { get; set; }
    public string agency_email { get; set; }
    public Agency(string[] keys, string[] values)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (this.GetType().GetProperty(keys[i]) == null) continue;
            switch (keys[i])
            {
                default:
                    this.GetType().GetProperty(keys[i]).SetValue(this, values[i], null);
                    break;
            }
        }
    }
}
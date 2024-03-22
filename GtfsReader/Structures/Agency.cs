namespace GtfsReader.Structures;

public class Agency
{
    public string agency_id { get; }
    public string agency_name { get; }
    public string agency_url { get; }
    public string agency_timezone { get; }
    public string agency_lang { get; }
    public string agency_phone { get; }

    public Agency(string agencyId, string agencyName, string agencyUrl, string agencyTimezone, string agencyLang, string agencyPhone)
    {
        agency_id = agencyId;
        agency_name = agencyName;
        agency_url = agencyUrl;
        agency_timezone = agencyTimezone;
        agency_lang = agencyLang;
        agency_phone = agencyPhone;
    }
}
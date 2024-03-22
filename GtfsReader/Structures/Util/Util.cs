using GtfsReader.Structures;

namespace GtfsReader.Util;

public static class Util
{
    public static string[] GetValuesFromCsvLine(string input)
    {
        string currentValue = "";
        bool isInQoutes = true;
        List<string> result = new List<string>();
        for (int i = 0; i < input.Length; i++)
        {
            currentValue += input[i];
            if (currentValue.Length == 1 && currentValue[0] == '"') isInQoutes = true;
            if (currentValue.Length == 1 && currentValue[0] != '"') isInQoutes = false;
            if (isInQoutes && currentValue[^1] == ',' && currentValue[^2] == '"' ||
                !isInQoutes && currentValue[^1] == ',' ||
                isInQoutes && i + 1 == input.Length ||
                !isInQoutes && i + 1 == input.Length )
            {
                result.Add(currentValue.Trim(',').Trim('"'));
                currentValue = "";
            }
        }

        return result.ToArray();
    }
    
    public static DateOnly CreateDateFromString(string input)
    {
        if (input == "") throw new ArgumentException();
        return DateOnly.Parse(input.Substring(0, 4) + "/" + input.Substring(4, 2) + "/" + input.Substring(6, 2));
    }
    
    public static string FormatStringToTimeString(string input)
    {
        if (input[1] != ':')
        {
            return int.Parse(input.Substring(0, 2)) % 24 + input.Substring(2);
        }
        else return input;
    }
    
    public static double CalculateDistance(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
    {
        double radiansOverDegrees = (Math.PI / 180.0);
        double sLatitudeRadians = sLatitude * radiansOverDegrees;
        double sLongitudeRadians = sLongitude * radiansOverDegrees;
        double eLatitudeRadians = eLatitude * radiansOverDegrees;
        double eLongitudeRadians = eLongitude * radiansOverDegrees;

        double dLongitude = eLongitudeRadians - sLongitudeRadians;
        double dLatitude = eLatitudeRadians - sLatitudeRadians;

        double result1 = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) + Math.Cos(sLatitudeRadians) * Math.Cos(eLatitudeRadians) * Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

        // Using 6371 as the radius of the earth in kilometers
        double result2 = 6371.0 * 2.0 * Math.Atan2(Math.Sqrt(result1), Math.Sqrt(1.0 - result1));

        return result2;
    }

    public static double CalculateDistance(Stop s, Stop e)
    {
        return CalculateDistance((double)s.stop_lat, (double)s.stop_lon, (double)e.stop_lat, (double)e.stop_lon);
    }
}
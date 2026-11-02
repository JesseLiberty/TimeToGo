using System.Globalization;
using System.Text.RegularExpressions;

namespace TimeToGo;

public static partial class TimeCalculator
{
    public const decimal WeeklyHours = 40m;

    // Accepts forms such as "8:30 am", "8:30am", "8:30 a.m.", "8 am", "830am", "14:15", "0830".
    [GeneratedRegex(@"^(?<h>\d{1,2}):?(?<m>\d{2})?\s*(?<ampm>a\.?m?\.?|p\.?m?\.?)?$", RegexOptions.IgnoreCase)]
    private static partial Regex TimePattern();

    public static bool TryParseArrival(string? text, out TimeSpan time)
    {
        time = default;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var match = TimePattern().Match(text.Trim());
        if (!match.Success)
            return false;

        int hour = int.Parse(match.Groups["h"].Value, CultureInfo.InvariantCulture);
        int minute = match.Groups["m"].Success
            ? int.Parse(match.Groups["m"].Value, CultureInfo.InvariantCulture)
            : 0;
        if (minute > 59)
            return false;

        if (match.Groups["ampm"].Success)
        {
            if (hour < 1 || hour > 12)
                return false;
            bool isPm = char.ToLowerInvariant(match.Groups["ampm"].Value[0]) == 'p';
            hour %= 12;
            if (isPm)
                hour += 12;
        }
        else if (hour > 23)
        {
            return false;
        }

        time = new TimeSpan(hour, minute, 0);
        return true;
    }

    public static bool TryParseHoursWorked(string? text, out decimal hours)
    {
        hours = 0;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (!decimal.TryParse(text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out hours) &&
            !decimal.TryParse(text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out hours))
            return false;

        return hours >= 0 && hours <= WeeklyHours;
    }

    /// <summary>Returns the departure time of day and how many days after the arrival day it falls.</summary>
    public static (TimeSpan TimeOfDay, int DaysLater) ComputeDeparture(TimeSpan arrival, decimal hoursWorked)
    {
        decimal remainingMinutes = Math.Round((WeeklyHours - hoursWorked) * 60m, MidpointRounding.AwayFromZero);
        var departure = arrival + TimeSpan.FromMinutes((double)remainingMinutes);
        return (new TimeSpan(departure.Hours, departure.Minutes, 0), departure.Days);
    }

    public static string FormatTwelveHour(TimeSpan timeOfDay) =>
        DateTime.Today.Add(timeOfDay).ToString("h:mm tt", CultureInfo.InvariantCulture).ToLowerInvariant();
}

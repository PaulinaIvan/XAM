using System.ComponentModel.DataAnnotations;

namespace XAM.Models;

public class DataHolder
{
    [Key] public int DataHolderId { get; set; }

    public List<Exam> Exams { get; set; } = new();

    public StatisticsHolder Statistics { get; set; } = new();

    public string? CurrentCocktail { get; set; }
    public DateTime? TimeUntilNextCocktail
    {
        get
        {
            if (_timeUntilNextCocktail.HasValue)
            {
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(_timeUntilNextCocktail.Value, localTimeZone);
                return localTime;
            }
            else
                return null;
        }
        set
        {
            if (value.HasValue)
            {
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(value.Value, localTimeZone);
                _timeUntilNextCocktail = utcTime;
            }
            else
                _timeUntilNextCocktail = null;
        }
    }
    private DateTime? _timeUntilNextCocktail;
}
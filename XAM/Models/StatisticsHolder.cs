using System.ComponentModel.DataAnnotations;

namespace XAM.Models;

public class StatisticsHolder
{
    [Key] public int StatisticsId { get; set; }

    public int LifetimeCreatedExamsCounter { get; set; }
    public int LifetimeCreatedFlashcardsCounter { get; set; }
    public int TodayCreatedExamsCounter { get; set; }
    public int TodayCreatedFlashcardsCounter { get; set; }
    public int TodayHighscoresBeatenCounter { get; set; }
    public int TodayChallengesTakenCounter { get; set; }

    public void ResetTodaysStatistics()
    {
        TodayCreatedExamsCounter = 0;
        TodayCreatedFlashcardsCounter = 0;
        TodayHighscoresBeatenCounter = 0;
        TodayChallengesTakenCounter = 0;
    }

    public bool IsEligibleForCocktail()
    {
        return TodayChallengesTakenCounter >= 2 && TodayHighscoresBeatenCounter >= 1;
    }
}
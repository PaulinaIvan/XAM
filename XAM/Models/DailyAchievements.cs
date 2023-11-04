namespace XAM.Models;

public class DailyAchievements
{
    public int ExamsCreated {get; set;}
    public int FlashcardsCreated {get; set;}
    public int HighscoresBeaten {get; set;}
    public int ChallengesTaken {get; set;}

    public DailyAchievements(int examsCreated, int flashcardsCreated, int highscoresBeaten, int challengesTaken)
    {
        ExamsCreated = examsCreated;
        FlashcardsCreated = flashcardsCreated;
        HighscoresBeaten = highscoresBeaten;
        ChallengesTaken = challengesTaken;
    }

    public void ResetAchievements()
    {
        ExamsCreated = 0;
        FlashcardsCreated = 0;
        HighscoresBeaten = 0;
        ChallengesTaken = 0;
    }
}
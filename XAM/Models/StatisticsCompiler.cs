namespace XAM.Models;

public class StatisticsCompiler : IStatisticsCompiler
{
    public object CompileStatistics(DataHolder dataHolder)
    {
        return new
        {
            lifetimeExams = dataHolder.Statistics.LifetimeCreatedExamsCounter,
            lifetimeFlashcards = dataHolder.Statistics.LifetimeCreatedFlashcardsCounter,
            todayExams = dataHolder.Statistics.TodayCreatedExamsCounter,
            todayFlashcards = dataHolder.Statistics.TodayCreatedFlashcardsCounter,
            todayChallengeHighscores = dataHolder.Statistics.TodayHighscoresBeatenCounter,
            todayChallengeAttempts = dataHolder.Statistics.TodayChallengesTakenCounter,
            challengeHighscoresList = dataHolder.Exams
                .Where(exam => exam.ChallengeHighscore > 0)
                .Select(exam => new { name = exam.Name, challengeHighscore = exam.ChallengeHighscore })
                .ToArray()
        };
    }
}

public interface IStatisticsCompiler
{
    object CompileStatistics(DataHolder dataHolder);
}
fetch(`/Statistics/FetchStatistics`)
.then(response => response.json())
.then(data => {
    if (data !== null)
    {
        document.getElementById("lifetimeExams").innerText += ` ${data.lifetimeExams}`;
        document.getElementById("lifetimeFlashcards").innerText += ` ${data.lifetimeFlashcards}`;
        document.getElementById("todayExams").innerText += ` ${data.todayExams}`;
        document.getElementById("todayFlashcards").innerText += ` ${data.todayFlashcards}`;
        document.getElementById("todayChallengeHighscores").innerText += ` ${data.todayChallengeHighscores}`;
        document.getElementById("todayChallengeAttempts").innerText += ` ${data.todayChallengeAttempts}`;
        if(data.challengeHighscoresList && Array.isArray(data.challengeHighscoresList) && data.challengeHighscoresList.length > 0)
        {
            data.challengeHighscoresList.forEach(examNameScore => {

                const examName = examNameScore.name;
                const score = examNameScore.challengeHighscore;

                document.getElementById("challengeHighscores").innerText += `\n${examName}: ${score}`;
            });
        }
        else
        {
            document.getElementById("challengeHighscores").innerText += `\nNo highscores yet.`;
        }
    }
    else
    {
        console.log('Invalid data format received from the server.');
    }
})
.catch(error => {
    console.error('Error:', error);
});
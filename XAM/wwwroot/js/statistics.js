fetch(`/Statistics/FetchStatistics`)
    .then(response => response.json())
    .then(data => {
        if (data !== null) {
            document.getElementById("lifetimeExams").innerText += ` ${data.lifetimeExams}`;
            document.getElementById("lifetimeFlashcards").innerText += ` ${data.lifetimeFlashcards}`;
            document.getElementById("todayExams").innerText += ` ${data.todayExams}`;
            document.getElementById("todayFlashcards").innerText += ` ${data.todayFlashcards}`;
            document.getElementById("todayChallengeHighscores").innerText += ` ${data.todayChallengeHighscores}`;
            document.getElementById("todayChallengeAttempts").innerText += ` ${data.todayChallengeAttempts}`;

            const table = document.getElementById("challengeHighscores");
            const tbody = table.querySelector("tbody") || document.createElement("tbody");

            // Clear existing rows
            tbody.innerHTML = '';

            if (data.challengeHighscoresList && Array.isArray(data.challengeHighscoresList) && data.challengeHighscoresList.length > 0) {
                data.challengeHighscoresList.forEach(examNameScore => {
                    const examName = examNameScore.name;
                    const score = examNameScore.challengeHighscore;

                    const row = tbody.insertRow();

                    const cell1 = row.insertCell(0);
                    const cell2 = row.insertCell(1);

                    cell1.innerText = examName;
                    cell2.innerText = score;
                });
            } else {
                // If there are no highscores, add a message row
                const row = tbody.insertRow();
                const cell = row.insertCell(0);
                cell.colSpan = 2;
                cell.innerText = 'No highscores yet.';
            }

            // Append the tbody to the table if it doesn't exist
            if (!table.querySelector("tbody")) {
                table.appendChild(tbody);
            }
        } else {
            console.log('Invalid data format received from the server.');
        }
    })
    .catch(error => {
        console.error('Error:', error);
    });

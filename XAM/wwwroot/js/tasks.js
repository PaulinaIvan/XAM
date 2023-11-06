fetch(`/Tasks/FetchExamNames`)
.then(response => response.json())
.then(allExamNames => {
    if (allExamNames && Array.isArray(allExamNames) && allExamNames.length > 0)
    {
        allExamNames.forEach(examName => {
            newOption = new Option(examName, `${examName}Exam`);
            document.getElementById("examsDropdown").add(newOption, undefined);
        });
    }
})
.catch(error => {
    console.error('Error:', error);
});

async function getFlashcardsFromExam(examName) {
    try {
        const response = await fetch(`/Tasks/FetchFlashcardsOfExam?examName=${examName}`);
        const data = await response.json();

        if (data.errorCode === "NoFlashcards")
        {
            alert(data.errorMessage);
            return data.errorCode;
        }
        else
        {
            return data;
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

async function setHighscore(examName, score) {
    try {
        const response = await fetch(`/Tasks/SetChallengeHighscoreForExam?examName=${examName}&score=${score}`);
        const data = await response.json();

        if (data.errorCode === "NoExamWithName")
        {
            alert(data.errorMessage);
            return data.errorMessage;
        }
        else
        {
            return data;
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

var challengeScore;
var currentQuestion;
var flashcards;
var challengeExamName;
async function startChallengeMode()
{
    const examDropdown = document.getElementById("examsDropdown");
    const examChosen = examDropdown.options[examDropdown.selectedIndex];
    if(examChosen.value === "default")
    {
        alert("Select an exam to challenge yourself on.");
        return;
    }

    resetChallengeWindow();
    flashcards = await getFlashcardsFromExam(examChosen.text);
    challengeExamName = examChosen.text;
    if(flashcards === "NoFlashcards")
    {
        return;
    }
    else
    {
        var dimmerBackground = document.getElementById("challengeModeDimmerBackground");
        dimmerBackground.style.display = "block";
        document.getElementById("challengeModeTitle").innerText = `${examChosen.text} challenge`;
        updateChallengeWindow();
    }
}

function stopChallengeMode()
{
    if(currentQuestion == -1)
    {
        closeChallengeMode();
    }
    else
    {
        currentQuestion = -1;
        displayEndScreen();
    }
}

function closeChallengeMode()
{
    var dimmerBackground = document.getElementById("challengeModeDimmerBackground");
    dimmerBackground.style.display = "none";
}

async function displayEndScreen()
{
    document.getElementById("gameFieldHolder").style.display = "none";
    document.getElementById("endScreenHolder").style.display = "block";

    const endMessage = await setHighscore(challengeExamName, challengeScore);

    document.getElementById("endMessageDiv").innerText = endMessage;
}

function resetChallengeWindow()
{
    challengeScore = 0;
    currentQuestion = 0;
    flashcards = null;
    examChosen = null;
    updateChallengeScore(0);

    document.getElementById("gameFieldHolder").style.display = "block";
    document.getElementById("endScreenHolder").style.display = "none";
}

function updateChallengeWindow()
{
    var flashcardBox = document.getElementById("flashcardBox");
    if(flashcardBox.classList.contains("flashcardBoxBack"))
    {
        flashcardBox.classList.remove("flashcardBoxBack");
        flashcardBox.classList.add("flashcardBoxFront");
    }
    document.getElementById("checkFlashcardAnswerButton").style.display = "block";
    document.getElementById("answerButtons").style.display = "none";

    if(currentQuestion < flashcards.length)
    {
        flashcardBox.innerText = `Q: ${flashcards[currentQuestion].frontText}`;
    }
    else
    {
        stopChallengeMode();
    }
}

function checkFlashcardAnswer()
{
    var flashcardBox = document.getElementById("flashcardBox");
    flashcardBox.classList.remove("flashcardBoxFront");
    flashcardBox.classList.add("flashcardBoxBack");

    flashcardBox.innerText = `A: ${flashcards[currentQuestion].backText}`;

    document.getElementById("checkFlashcardAnswerButton").style.display = 'none';
    document.getElementById("answerButtons").style.display = 'block';
}

function guessedAnswer()
{
    challengeScore += 1;
    updateChallengeScore(challengeScore);

    ++currentQuestion;
    updateChallengeWindow();
}

function forgotAnswer()
{
    stopChallengeMode();
}

function updateChallengeScore(score)
{
    document.getElementById("challengeModeScoreDisplay").innerText = `Score: ${score}`;
}
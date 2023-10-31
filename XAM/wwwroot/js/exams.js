fetch(`/Exam/FetchExams`)
.then(response => response.json())
.then(data => {
    if (data && Array.isArray(data) && data.length > 0)
    {
        data.forEach(exam => {
            addExam(exam.name, exam.date.substring(0, 10));
            exam.flashcards.forEach(flashcard => {
                addFlashcard(flashcard.frontText, flashcard.backText, exam.name);
            });
        });
    }
})
.catch(error => {
    console.error('Error:', error);
});
function createExam()
{
    const examNameValue = document.getElementById('examName').value;
    const examDateValue = document.getElementById('examDate').value;

    if(examNameValue === '' || examDateValue === '')
    {
        alert("Please enter details!");
        return;
    }

    if(!document.getElementById(`${examNameValue}Grid`))
    {
        fetch(`/Home/CreateExam?name=${examNameValue}&date=${examDateValue}`)
            .then(response => response.json())
            .then(data => {
                if(data.errorCode === 'BadName')
                {
                    alert('Use only letters, numbers and spaces in exam name.');
                    throw new Error(data.errorMessage);
                }
                else if(data.errorCode === 'BadDate')
                {
                    alert('Invalid date.');
                    throw new Error(data.errorMessage);
                }
                else
                {
                    addExam(data.name, data.date);
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }
    else
    {
        alert("Exam name already exists!");
    }

    document.getElementById('examName').value = '';
    document.getElementById('examDate').value = '';
}

function addExam(examNameValue, dateStringValue)
{
    const examGrid = document.getElementById('exam-grid');

    const cardControls = document.createElement('div');

    const frontTextInput = document.createElement('input');
    frontTextInput.setAttribute('placeholder', 'Question');
    cardControls.appendChild(frontTextInput);

    const backTextInput = document.createElement('input');
    backTextInput.setAttribute('placeholder', 'Answer');
    cardControls.appendChild(backTextInput);

    const createFlashcardButton = document.createElement('button');
    createFlashcardButton.innerHTML = "Add Flashcard";
    cardControls.appendChild(createFlashcardButton);

    const deleteExamButton = document.createElement('button');
    deleteExamButton.innerHTML = "Delete Exam";
    cardControls.appendChild(deleteExamButton);

    const flashcardGrid = document.createElement('div');
    flashcardGrid.classList.add('flashcard-grid');

    const examBox = document.createElement('div');
    examBox.classList.add("exam-box");
    examBox.textContent = `${examNameValue} - ${dateStringValue}`;
    examBox.appendChild(cardControls);

    const examTimer = document.createElement('div');
    generateExamTimerValue(dateStringValue, examTimer);
    setInterval(() => generateExamTimerValue(dateStringValue, examTimer), 1000);

    createFlashcardButton.onclick = function() {
        createFlashcard(frontTextInput, backTextInput, examNameValue);
    };

    deleteExamButton.onclick = function() {
        deleteExam(examNameValue);
    };

    examBox.appendChild(cardControls);
    examBox.appendChild(flashcardGrid);
    examBox.appendChild(examTimer);
    examGrid.appendChild(examBox);
    examBox.setAttribute('id', `${examNameValue}Id`);
    flashcardGrid.setAttribute('id', `${examNameValue}Grid`);
}

function generateExamTimerValue(dateStringValue, locationDiv) 
{
    const currentDate = new Date();
    const examDate = new Date(dateStringValue);

    const timeDifference = examDate - currentDate - 3 * 1000 * 3600;
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));
    const hours = Math.floor((timeDifference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((timeDifference % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((timeDifference % (1000 * 60)) / 1000);

    // Text
    var timeLeftUntilExam = 'Time is up :)';
    if(timeDifference > 0)
    {
        timeLeftUntilExam = `${days}d ${hours}h ${minutes}m ${seconds}s`;
    }

    locationDiv.textContent = timeLeftUntilExam;

    // Color

    const blackColor = [0, 0, 0];
    const redColor = [255, 0, 0];

    const textColor = lerpColor(redColor, blackColor, clamp(timeDifference / (1000 * 3600 * 24 * 7), 0, 1));

    locationDiv.style.color = textColor;
}

function lerpColor(color1, color2, t)
{
    const r = Math.round(color1[0] + t * (color2[0] - color1[0]));
    const g = Math.round(color1[1] + t * (color2[1] - color1[1]));
    const b = Math.round(color1[2] + t * (color2[2] - color1[2]));
    return `rgb(${r},${g},${b})`;
}

function clamp(value, min, max)
{
    return Math.min(Math.max(value, min), max);
}

// Download
document.getElementById('downloadButton').addEventListener('click', function () {
    fetch('/Home/DownloadAllData')
        .then(response => {
            if (response.ok) {
                return response.blob();
            } else {
                throw new Error('Failed to download exams.');
            }
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'XamData.json';
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => console.error('Error:', error));
});

// Browse button
document.getElementById('fileInput').addEventListener('change', function () {
    const fileInput = document.getElementById('fileInput');
    const file = fileInput.files[0];
    
    if (file) {
        const formData = new FormData();
        formData.append('file', file);
    
        fetch('/Home/UploadDataFile', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.list && Array.isArray(data.list)) {
    
                    data.list.forEach(exam => {
                        addExam(exam.name, exam.date.substring(0, 10));
                        exam.flashcards.forEach(flashcard => {
                            addFlashcard(flashcard.frontText, flashcard.backText, exam.name);
                        });
                    });
                }
                else {
                    console.error('Invalid data format received from the server.');
                }
            })
            .catch(error => console.error('Error:', error));
    }
    else {
        alert('Please select a file to upload.');
    }
    });
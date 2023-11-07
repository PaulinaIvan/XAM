fetch(`/Preparation/FetchExams`)
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
        fetch(`/Preparation/CreateExam?name=${examNameValue}&date=${examDateValue}`)
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

function createFlashcard(frontTextInput, backTextInput, examNameValue)
{
    fetch(`/Preparation/CreateFlashcard?frontText=${frontTextInput.value}&backText=${backTextInput.value}&examName=${examNameValue}`)
        .then(response => response.json())
        .then(data => {
            addFlashcard(data.frontText, data.backText, data.examName);
        })
        .catch(error => {
            console.error('Error:', error);
        });

    frontTextInput.value = '';
    backTextInput.value = '';
}

function truncateText(text, maxLength) {
    if (text.length <= maxLength) {
        return text;
    } else {
        return text.substring(0, maxLength) + '...';
    }
}

function createExpandedTextBox(fullText) {
    const textBox = document.createElement('div');
    textBox.classList.add('expanded-text-box');
    textBox.textContent = fullText;

    textBox.addEventListener('click', function (event) {
        event.stopPropagation();
    });

    document.body.appendChild(textBox);
    document.body.addEventListener('click', function () {
        textBox.style.display = 'none';
    });

    return textBox;
}

let currentlyOpenTextBox = null;

function addFlashcard(frontTextValue, backTextValue, examNameValue) {

    const box = document.createElement('div');
    box.classList.add('box');

    const truncatedFrontText = truncateText(frontTextValue, 20);
    const truncatedBackText = truncateText(backTextValue, 20);

    const boxInner = document.createElement('div');
    boxInner.classList.add('box-inner');

    const boxFront = document.createElement('div');
    boxFront.classList.add('box-front');
    boxFront.textContent = truncatedFrontText;

    const boxBack = document.createElement('div');
    boxBack.classList.add('box-back');
    boxBack.textContent = truncatedBackText;

    const deleteButton = document.createElement('button');
    deleteButton.classList.add('delete-button');
    deleteButton.textContent = "X";

    const moreButton = document.createElement('button');
    moreButton.classList.add('more-button');
    moreButton.textContent = "i";

    let isFlipped = false;

    deleteButton.addEventListener('click', () => {
        deleteFlashcard(examNameValue, box);
    });

    moreButton.addEventListener('click', function (event) {
        event.stopPropagation();
        if (currentlyOpenTextBox) {
            currentlyOpenTextBox.style.display = 'none';
        }

        const textToShow = isFlipped ? ("Answer:\n" + backTextValue) : ("Question:\n" + frontTextValue);
        const textBox = createExpandedTextBox(textToShow);
        textBox.style.display = 'block';

        currentlyOpenTextBox = textBox;
    });

    box.addEventListener('click', () => {
        isFlipped = !isFlipped;
        boxInner.style.transform = isFlipped ? 'rotateY(180deg)' : 'rotateY(0deg)';
    });

    boxInner.appendChild(boxFront);
    boxInner.appendChild(boxBack);
    boxInner.appendChild(deleteButton);
    boxInner.appendChild(moreButton);
    box.appendChild(boxInner);
    document.getElementById(`${examNameValue}Grid`).appendChild(box);
}

function deleteFlashcard(examNameValue, flashcardElement) {
    // Find the parent container of the flashcard
    const flashcardGrid = document.getElementById(`${examNameValue}Grid`);

    if (flashcardGrid && flashcardElement) {
        // Get the index of the flashcard within its parent container
        const flashcardIndex = Array.from(flashcardGrid.children).indexOf(flashcardElement);

        // Remove the flashcard element from its parent
        flashcardElement.remove();

        // Send an AJAX request to delete the flashcard from the server
        const xhr = new XMLHttpRequest();
        xhr.open('DELETE', `/Preparation/DeleteFlashcard?examName=${examNameValue}&flashcardIndex=${flashcardIndex}`);
        xhr.send();
    }
}

function deleteExam(examNameValue)
{
    const examBox = document.getElementById(`${examNameValue}Id`);
    fetch(`/Preparation/DeleteExam?examName=${examNameValue}`)
        .then(response => response.json())
        .then(data => {
            examBox.remove();
        })
        .catch(error => {
            console.error('Error:', error);
        });
    const fileInput = document.getElementById('fileInput');
    fileInput.value = ''; // Clear the file input value
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

// Save to database
document.getElementById('saveToDatabaseButton').addEventListener('click', function () {
    fetch('/Home/SaveToDatabase')
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Failed to save to database.');
            }
        })
        .then(data => {
            console.log(`Database save successful!: ${data}`);
            alert(`Database save successful!`);
        })
        .catch(error => console.error('Error:', error));
});

// Download
document.getElementById('downloadButton').addEventListener('click', function () {
    fetch('/Preparation/DownloadAllData')
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

    fetch('/Preparation/UploadDataFile', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            console.log(data);
            if (data && Array.isArray(data)) {

                data.forEach(exam => {
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
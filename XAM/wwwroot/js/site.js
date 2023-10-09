//PREPARATION.CSHTML

fetch(`/Home/FetchExams`)
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
    else
    {
        console.log('Invalid data format received from the server.');
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

function addExam(examNameValue, dateNameValue)
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
    examBox.textContent = `${examNameValue} - ${dateNameValue}`;
    examBox.appendChild(cardControls);

    const examTimer = document.createElement('div');    //create the timer
    setInterval(() => examTimer.textContent = generateExamTimerValue(dateNameValue), 1000);  //change the value of the timer every second

    createFlashcardButton.onclick = function() {
        createFlashcard(frontTextInput, backTextInput, examNameValue);
    };

    deleteExamButton.onclick = function() {
        deleteExam(examNameValue);
    };

    examBox.appendChild(cardControls);
    examBox.appendChild(flashcardGrid);
    examBox.appendChild(examTimer);     //add the timer
    examGrid.appendChild(examBox);
    examBox.setAttribute('id', `${examNameValue}Id`);
    flashcardGrid.setAttribute('id', `${examNameValue}Grid`);
}

function createFlashcard(frontTextInput, backTextInput, examNameValue)
{
    fetch(`/Home/CreateFlashcard?frontText=${frontTextInput.value}&backText=${backTextInput.value}&examName=${examNameValue}`)
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

function addFlashcard(frontTextValue, backTextValue, examNameValue) {
    // Container for flashcard + delete button
    const flashcardContainer = document.createElement('div');
    flashcardContainer.classList.add('flashcard-container');

    const box = document.createElement('div');
    box.classList.add('box');

    const boxInner = document.createElement('div');
    boxInner.classList.add('box-inner');

    const boxFront = document.createElement('div');
    boxFront.classList.add('box-front');
    boxFront.textContent = frontTextValue;

    const boxBack = document.createElement('div');
    boxBack.classList.add('box-back');
    boxBack.textContent = backTextValue;

    const deleteFlashcardButton = document.createElement('button');
    deleteFlashcardButton.innerHTML = "Delete";
    deleteFlashcardButton.addEventListener('click', () => {
        deleteFlashcard(flashcardContainer, frontTextValue, backTextValue, examNameValue);
    });

    box.addEventListener('click', () => {
        boxInner.style.transform = (boxInner.style.transform === 'rotateY(180deg)' ? 'rotateY(0deg)' : 'rotateY(180deg)');
    });

    boxInner.appendChild(boxFront);
    boxInner.appendChild(boxBack);
    box.appendChild(boxInner);

    flashcardContainer.appendChild(box);
    flashcardContainer.appendChild(deleteFlashcardButton);
    
    document.getElementById(`${examNameValue}Grid`).appendChild(flashcardContainer);
}

function deleteFlashcard(flashcardContainer, frontTextValue, backTextValue, examNameValue) {
    fetch(`/Home/DeleteFlashcard?frontText=${frontTextValue}&examName=${examNameValue}`)
    .then(response => {
        if(response.ok) {
            flashcardContainer.remove();
        } else {
            return response.json();
        }
    })
    .then(data => {
        if(data && data.message) {
            console.error(data.message);
        }
    })
    .catch(error => console.error('Error:', error));
}

function deleteExam(examNameValue)
{
    const examBox = document.getElementById(`${examNameValue}Id`);
    fetch(`/Home/DeleteExam?examName=${examNameValue}`)
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

function generateExamTimerValue(dateNameValue) 
{
    const currentDate = new Date();
    const examDate = new Date(dateNameValue);

    const timeDifference = examDate - currentDate - 3 * 1000 * 3600;    //adjusted for timezone need to do it for all timezones or use a library
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));
    const hours = Math.floor((timeDifference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((timeDifference % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((timeDifference % (1000 * 60)) / 1000);

    const timeLeftUntilExam = `${days}d ${hours}h ${minutes}m ${seconds}s`;
    return timeLeftUntilExam;
}

// Download
document.getElementById('downloadButton').addEventListener('click', function() {
fetch('/Home/GetAllExams')
.then(response => response.json())
.then(exams => {
    if(exams && Array.isArray(exams) && exams.length > 0)
    {
        saveExamsToFile(exams);
    }
    else
    {
        alert("Nothing to download.")
    }
})
.catch(error => console.error('Error:', error));
});

function saveExamsToFile(exams)
{
    const formattedExams = exams.map(exam => {
        return {
            ...exam,
            date: exam.date.substring(0, 10)
        };
    });

    const jsonContent = JSON.stringify(formattedExams);
    const blob = new Blob([jsonContent], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'ExamData.json';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

// Browse button
document.getElementById('fileInput').addEventListener('change', function () {
const fileInput = document.getElementById('fileInput');
const file = fileInput.files[0];

if (file) {
    const formData = new FormData();
    formData.append('file', file);

    fetch('/Home/UploadExamFile', {
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
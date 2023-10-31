
fetch(`/Flashcards/FetchExams`)
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
        xhr.open('DELETE', `/Home/DeleteFlashcard?examName=${examNameValue}&flashcardIndex=${flashcardIndex}`);
        xhr.send();
    }
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





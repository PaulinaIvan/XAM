// Save to database
document.getElementById('saveToDatabaseButton').addEventListener('click', function () {
    fetch('/Home/SaveToDatabaseAction')
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Failed to save to database.');
            }
        })
        .then(data => {
            console.log(data);
            alert(data);
        })
        .catch(error => console.error('Error:', error));
});
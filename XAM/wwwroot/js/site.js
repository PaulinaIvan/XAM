document.addEventListener('click', function(event) {
    fetch('/Home/DownloadAllData')
    .then(response => {
        if (response.ok) {
            return response.blob();
        } else {
            throw new Error('Failed to download exams.');
        }
    })
    .then(blob => {
        const reader = new FileReader();

        reader.onload = function() {
            const jsonString = reader.result;
            //console.log(jsonString);

            var user = sessionStorage.getItem("currentUser");
            localStorage.setItem(user,jsonString);
        };
        reader.readAsText(blob);
    })
    .catch(error => console.error('Error:', error));

});
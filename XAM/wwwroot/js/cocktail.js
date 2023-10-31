document.getElementById('cocktailButton').addEventListener('click', function () {
    fetch('/Home/GetRandomCocktail')
        .then(response => response.json())
        .then(data => {
            if (data && data.drinks && data.drinks.length > 0) {
                const cocktailName = data.drinks[0].strDrink;
                document.getElementById('cocktailName').textContent = `Cocktail Name: ${cocktailName}`;
            } else {
                console.error('Invalid data format received from the server.');
            }
        })
        .catch(error => console.error('Error:', error));
});
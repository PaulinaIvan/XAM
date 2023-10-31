document.getElementById('cocktailButton').addEventListener('click', function () {
    fetch(`/Home/FetchStatistics`)
    .then(response => response.json())
    .then(data => {
        if (data !== null)
        {
            if (data.lifetimeExams > 0) {
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
            }
            else {
                document.getElementById('cocktailName').textContent = `You haven't worked hard enough to deserve a cocktail yet!`;
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

});
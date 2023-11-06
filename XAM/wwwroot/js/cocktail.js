document.getElementById('cocktailButton').addEventListener('click', function () {
    fetch('/Reward/FetchCocktail')
    .then(response => response.json())
    .then(data => {

        const notEligibleMessage = document.getElementById('notEligibleMessage');
        const cocktailCard = document.querySelector('.cocktail-card');

        if (data.errorCode === "NotEligible")
        {
            notEligibleMessage.style.display = 'block';
            cocktailCard.style.display = 'none';
        }
        else if (data.errorCode === "NoCocktailAvailable")
        {
            alert(data.errorMessage);
        }
        else if (data && data.drinks && data.drinks.length > 0) {
            const cocktailData = data.drinks[0];

            document.getElementById('cocktailName').textContent = `Cocktail Name: ${cocktailData.strDrink}`;

            const cocktailImage = document.getElementById('cocktailImage');
            cocktailImage.src = cocktailData.strDrinkThumb;
            cocktailImage.alt = `Image of ${cocktailData.strDrink}`;

            const ingredientsList = document.getElementById('ingredientsList');
            ingredientsList.innerHTML = '';
            for (let i = 1; i <= 15; i++) {
                const ingredient = cocktailData[`strIngredient${i}`];
                const measure = cocktailData[`strMeasure${i}`];
                if (ingredient && measure) {
                    const listItem = document.createElement('li');
                    listItem.textContent = `${measure} ${ingredient}`;
                    ingredientsList.appendChild(listItem);
                }
            }
            document.getElementById('instructions').textContent = cocktailData.strInstructions;
            notEligibleMessage.style.display = 'none';
            cocktailCard.style.display = 'block';
        }
        else
        {
            console.error('Invalid data format received from the server.');
        }
    })
    .catch(error => console.error('Error:', error));
});
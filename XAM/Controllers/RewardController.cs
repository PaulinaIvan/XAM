using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace XAM.Controllers;

public class RewardController : Controller
{
    private readonly DataHolder _dataHolder;
    private readonly HttpClient _client;

    public RewardController(DataHolder dataHolder, HttpClient client)
    {
        _dataHolder = dataHolder;
        _client = client;

        Task CocktailProducerTask = Task.Run(() => CocktailWaiter(_client));
    }

    public IActionResult Reward()
    {
        return View();
    }

    public IActionResult FetchCocktail()
    {
        if(!_dataHolder.Statistics.IsEligibleForCocktail())
        {
            ErrorRecord errorResponse = CreateErrorResponse("NotEligible", "You're not productive enough for the cocktail...");
            return Json(errorResponse);
        }

        if (string.IsNullOrEmpty(_dataHolder.CurrentCocktail))
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoCocktailAvailable", $"Cocktail API is having issues, please try again at {_dataHolder.TimeUntilNextCocktail}.");
            return Json(errorResponse);
        }

        return Content(_dataHolder.CurrentCocktail, "application/json");
    }

    private async Task CocktailWaiter(HttpClient client)
    {
        if (_dataHolder.TimeUntilNextCocktail == null || _dataHolder.TimeUntilNextCocktail < DateTime.Now)
        {
            if(_dataHolder.CurrentCocktail != null)
                _dataHolder.Statistics.ResetTodaysStatistics(); // This should be in its own time tracing async method

            _dataHolder.TimeUntilNextCocktail = DateTime.Now.Date.AddDays(1);
            
            try
            {
                _dataHolder.CurrentCocktail = await GetRandomCocktail(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _dataHolder.CurrentCocktail = null;
            }
            
        }
        else
        {
            await Task.Delay(_dataHolder.TimeUntilNextCocktail.Value.Subtract(DateTime.Now));
        }
    }

    private static async Task<string> GetRandomCocktail(HttpClient client)
    {
        try
        {
            string apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/random.php";
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception($"Request failed: {response.ReasonPhrase}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while fetching data from the Cocktail API: {ex.StackTrace}");
        }
    }
}

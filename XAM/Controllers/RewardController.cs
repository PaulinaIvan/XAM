using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace XAM.Controllers;

public class RewardController : Controller
{
    private readonly DataHolder _dataHolder;

    public RewardController(DataHolder dataHolder)
    {
        _dataHolder = dataHolder;

        Task CocktailProducerTask = Task.Run(() => CocktailWaiter(CancellationToken.None));
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
    
        string? todaysCocktail = _dataHolder.CurrentCocktail;
        if (string.IsNullOrEmpty(todaysCocktail))
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoCocktailAvailable", $"Please wait for the next cocktail in {_dataHolder.TimeUntilNextCocktail}.");
            return Json(errorResponse);
        }

        return Content(todaysCocktail, "application/json");
    }

    private async Task CocktailWaiter(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_dataHolder.TimeUntilNextCocktail == null || _dataHolder.TimeUntilNextCocktail < DateTime.Now)
            {
                _dataHolder.TimeUntilNextCocktail = DateTime.Now.Date.AddDays(1);

                string? newCocktail = null;
                try
                {
                    newCocktail = await GetRandomCocktail();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                _dataHolder.CurrentCocktail = newCocktail;
                _dataHolder.Statistics.ResetTodaysStatistics(); // This should be in its own time tracing async method
            }
            else
            {
                await Task.Delay(_dataHolder.TimeUntilNextCocktail.Value.Subtract(DateTime.Now), stoppingToken);
            }
        }
    }

    private static async Task<string> GetRandomCocktail()
    {
        try
        {
            using (HttpClient client = new())
            {
                string apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/random.php";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    _ = new Exception($"API request failed: {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            _ = new Exception($"An error occurred while fetching data from the API: {ex.Message}");
        }

        return "";
    }
}

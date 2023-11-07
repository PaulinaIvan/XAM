using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace XAM.Controllers;

public class RewardController : Controller
{
    private readonly XamDbContext _context;

    public RewardController(XamDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Reward(HttpClient httpClient)
    {
        DataHolder dataHolder = _context.GetDataHolder();
        if (dataHolder.TimeUntilNextCocktail == null || dataHolder.TimeUntilNextCocktail < DateTime.Now)
        {
            if(dataHolder.CurrentCocktail != null)
                dataHolder.Statistics.ResetTodaysStatistics(); // This should be in its own time tracing async method

            dataHolder.TimeUntilNextCocktail = DateTime.Now.Date.AddDays(1);
            
            try
            {
                dataHolder.CurrentCocktail = await GetRandomCocktail(httpClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                dataHolder.CurrentCocktail = null;
            }
            _context.SaveToDatabase(dataHolder);
        }
        return View();
    }

    public IActionResult FetchCocktail()
    {
        DataHolder dataHolder = _context.GetDataHolder();
        if(!dataHolder.Statistics.IsEligibleForCocktail())
        {
            ErrorRecord errorResponse = CreateErrorResponse("NotEligible", "You're not productive enough for the cocktail...");
            return Json(errorResponse);
        }

        if (string.IsNullOrEmpty(dataHolder.CurrentCocktail))
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoCocktailAvailable", $"Cocktail API is having issues, please try again at {dataHolder.TimeUntilNextCocktail}.");
            return Json(errorResponse);
        }

        return Content(dataHolder.CurrentCocktail, "application/json");
    }

    private static async Task<string> GetRandomCocktail(HttpClient httpClient)
    {
        try
        {
            string apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/random.php";
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

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

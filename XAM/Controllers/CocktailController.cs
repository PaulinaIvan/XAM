using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XAM.Models;

namespace XAM.Controllers;

public class CocktailController : Controller
{
    private readonly DataHolder _dataHolder;

    public CocktailController(DataHolder dataHolder)
    {
        _dataHolder = dataHolder;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRandomCocktail()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://www.thecocktaildb.com/api/json/v1/1/random.php";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    _dataHolder.TodaysCocktail = jsonContent;

                    return NoContent();
                }
                else
                {
                    return BadRequest("API request failed: " + response.ReasonPhrase);
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching data from the API.", error = ex.Message });
        }
    }


}
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using XAM.Controllers;
using XAM.Models;


public class CocktailGenerator : BackgroundService
{
    private readonly CocktailController _cocktailController;
    private readonly DataHolder _dataHolder;

    public CocktailGenerator(CocktailController cocktailController, DataHolder dataHolder)
    {
        _cocktailController = cocktailController;
        _dataHolder = dataHolder;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) // 7. Usage of async/await
    {
        await _cocktailController.GetRandomCocktail(); //Generate a cocktail at startup
        Console.WriteLine("CocktailGenerator is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var next4PM = now.Date.AddHours(16); 
            if (now >= next4PM)
            {
                next4PM = next4PM.AddDays(1); // Move to the next day if it's already past 4 PM today
            }

            var delay = next4PM - now;
            await Task.Delay(delay, stoppingToken);

            Console.WriteLine("It's 4 PM, time to generate a cocktail!");
            await _cocktailController.GetRandomCocktail();
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(cancellationToken);
    }
}
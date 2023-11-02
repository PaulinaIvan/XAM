using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public class CocktailGenerator : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Calculate the delay until the next 4 PM
            var now = DateTime.Now;
            var next4PM = now.Date.AddHours(16); // 4 PM
            if (now >= next4PM)
            {
                next4PM = next4PM.AddDays(1); // Move to the next day if it's already past 4 PM today
            }

            var delay = next4PM - now;

            // Wait for the calculated delay
            await Task.Delay(delay, stoppingToken);

            Console.WriteLine("It's 4 PM, time to generate a cocktail!");
            //GetRandomCocktail();
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(cancellationToken);
    }
}
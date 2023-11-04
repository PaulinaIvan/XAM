using Microsoft.EntityFrameworkCore;
using XAM.Controllers;
using XAM.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<CocktailController>();
builder.Services.AddHostedService<CocktailGenerator>();
builder.Services.AddScoped<ErrorViewModel>();

builder.Services.AddDbContext<XamDbContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("xamDatabaseConnection");
    if(connectionString == null)
        throw new Exception("xamDatabaseConnection not configured in appsettings.Development.json");
    else
        options.UseNpgsql(connectionString);
});

builder.Services.AddSingleton(serviceProvider =>
{
    using var scope = serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<XamDbContext>();

    var dataHolder = dbContext.DataHolders
        .Include(dh => dh.Exams)
        .ThenInclude(exam => exam.Flashcards)
        .FirstOrDefault();

    if(dataHolder == null)
    {
        dataHolder = new DataHolder();

        dbContext.DataHolders.Add(dataHolder);
        dbContext.SaveChanges();
    }

    Console.WriteLine("Data loaded from database:");
    Console.WriteLine($"DataHolder Id: {dataHolder.DataHolderId}");
    Console.WriteLine($"Exam cound: {dataHolder.Exams.Count}");

    return dataHolder;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Other/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Other}/{action=Index}/{id?}");

app.Run();

// Get an instance of the CocktailGenerator
var cocktailGenerator = app.Services.GetRequiredService<CocktailGenerator>();

// Start the CocktailGenerator explicitly
cocktailGenerator.StartAsync(CancellationToken.None).GetAwaiter().GetResult();



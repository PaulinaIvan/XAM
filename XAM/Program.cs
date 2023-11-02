using XAM.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DataHolder>();

// Register CocktailGenerator as a hosted service
builder.Services.AddHostedService<CocktailGenerator>();

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



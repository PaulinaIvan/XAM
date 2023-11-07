using Microsoft.EntityFrameworkCore;
using XAM.Models;

namespace XAM;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddScoped<ErrorViewModel>();
        services.AddSingleton<HttpClient>();
        services.AddDbContext<XamDbContext>(options =>
        {
            string? connectionString = Configuration.GetConnectionString("xamDatabaseConnection");
            if(connectionString == null)
                throw new Exception("xamDatabaseConnection not configured in appsettings.Development.json");
            else
                options.UseNpgsql(connectionString);
        });
        services.AddSingleton(serviceProvider =>
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<XamDbContext>();

            var dataHolder = dbContext.DataHoldersTable
                .Include(dh => dh.Exams)
                    .ThenInclude(exam => exam.Flashcards)
                .Include(dh => dh.Statistics)
                .FirstOrDefault();

            if(dataHolder == null)
            {
                dataHolder = new DataHolder();

                dbContext.DataHoldersTable.Add(dataHolder);
                dbContext.SaveChanges();
            }

            return dataHolder;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Other/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
    }
}

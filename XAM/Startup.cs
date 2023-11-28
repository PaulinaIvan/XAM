using Microsoft.EntityFrameworkCore;
using XAM.Models;
using XAM.Middleware;
using MaxMind.GeoIP2;

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
        services.AddSession();
        services.AddControllersWithViews();
        services.AddHttpContextAccessor();
        services.AddScoped<ErrorViewModel>();
        services.AddSingleton(options =>
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "GeoLite2-Country.mmdb");
            if(!File.Exists(dbPath))
                throw new Exception("GeoLite2-Country.mmdb not found");
            else
                return new DatabaseReader(dbPath);
            
        });
        services.AddScoped<HttpClient>();
        services.AddDbContext<XamDbContext>(options =>
        {
            string? connectionString = Configuration.GetConnectionString("xamDatabaseConnection");
            if(connectionString == null)
                throw new Exception("xamDatabaseConnection not configured in appsettings.Development.json");
            else
                options.UseNpgsql(connectionString);
        }, ServiceLifetime.Scoped);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Shared/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthorization();
    }
}

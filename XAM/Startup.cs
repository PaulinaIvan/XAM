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
        services.AddSession();
        services.AddControllersWithViews();
        services.AddHttpContextAccessor();
        services.AddScoped<ErrorViewModel>();
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
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.UseSession();
    }
}

using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Hangfire;
using NToastNotify;

namespace Basecode.WebApp;

public partial class StartupWebApp
{
    public StartupWebApp(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureDependencies(services); // Configuration for dependency injections           
        ConfigureDatabase(services); // Configuration for database connections
        ConfigureMapper(services); // Configuration for entity model and view model mapping
        ConfigureCors(services); // Configuration for CORS
        ConfigureAuth(services); // Configuration for Authentication logic
        ConfigureMVC(services); // Configuration for MVC                  

        // Add services to the container.
        services.AddControllersWithViews();
        services.AddScoped<IEmailService, EmailService>();

        // Add Hangfire services.
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        services.AddMvc(options => { options.EnableEndpointRouting = false; });
        services.AddMvc().AddNToastNotifyToastr(new ToastrOptions
        {
            ProgressBar = true,
            PositionClass = ToastPositions.TopRight,
            CloseButton = true
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error"); // Enables site to redirect to page when an exception occurs
            app.UseHsts(); // Enables the Strict-Transport-Security header.
        }

        app.UseStaticFiles(); // Enables the use of static files
        app.UseHttpsRedirection(); // Enables redirection of HTTP to HTTPS requests.
        app.UseCors("CorsPolicy"); // Enables CORS
        app.UseRouting();
        app.UseAuthentication(); // Enables the ConfigureAuth service.
        app.UseAuthorization();
        app.UseHangfireDashboard();
        app.UseNToastNotify();


        ConfigureRoutes(app); // Configuration for API controller routing
        ConfigureAuth(app); // Configuration for Token Authentication


        // Shortlisting of Applicants that runs every 2 weeks
        //RecurringJob.AddOrUpdate<IShortlistingService>("shortlisting", service => service.ShortlistApplications(), "0 8 1,15 * *");
        // FOR TESTING ONLY: run shortlisting method every minute
        //RecurringJob.AddOrUpdate<IShortlistingService>("shortlisting", service => service.ShortlistApplications(), Cron.Minutely);

        /// <summary>
        /// Sets up a recurring Hangfire job to send automated reminders using the IEmailSendingService.
        /// The job will trigger every 2 weeks.
        /// </summary>
        RecurringJob.AddOrUpdate<IEmailSendingService>("auto-reminder", service => service.SendAutomatedReminder(),
            "0 0 7 * *");
    }
}
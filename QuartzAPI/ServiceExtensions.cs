using Microsoft.EntityFrameworkCore;
using Quartz;
using QuartzAPI.Jobs;
using QuartzAPI.Services;
using QuartzAPI.Services.ExceptionLoggerService;
using QuartzData;
using QuartzData.Context;
using QuartzData.Repositories;
using Serilog;

namespace QuartzAPI
{
    public static class ServiceExtensions
    {


        public static IServiceCollection AddDataContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection connection string is not configured");

            services.AddDbContext<QuartzDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            return services;
        }

        public static void AddHttpServices(this IServiceCollection services)
        {
            services.AddHttpClient<IHttpApiService, HttpApiService>();
        }
        public static void AddServices(this IServiceCollection services)
        {
            // Add application services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IHttpApiService, HttpApiService>();

            services.AddScoped<IJobStorageService, DatabaseJobStorageService>();


            services.AddScoped<IQuartzJobService, QuartzJobService>();

            services.AddScoped<IExceptionLoggerService, ExceptionLoggerService>();

            services.AddHttpContextAccessor();

            // For In-Memory without database: Uncomment the line below
            // builder.Services.AddScoped<IJobStorageService, InMemoryJobStorageService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IJobRepository, JobRepository>();
          
        }

        public static void AddSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        }

        public static void AddCors(this IServiceCollection services, IConfiguration configuration, string policyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                    b =>
                    {
                        var allowedOrigins = configuration["Cors"] ?? throw new ArgumentNullException("Cors is not configured is appsettings.json");

                        b.WithOrigins(allowedOrigins.Split(","))
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                    });
            });
        }

        public static void AddQuartz(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

                q.UseMicrosoftDependencyInjectionJobFactory();

                q.ScheduleJob<JobRescheduler>(trigger => trigger
                    .WithIdentity("RescheduleJob", "system")
                    .StartNow()
                    .WithDescription("Loads jobs from database into memory")
                );
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
                options.AwaitApplicationStarted = true;
            });
        }


    }
}

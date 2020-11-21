using System;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Linq;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicketNotifier.Checkers.Implementations;
using TicketNotifier.Checkers.Interfaces;
using TicketNotifier.Data;
using TicketNotifier.Repositories.Implementations;
using TicketNotifier.Repositories.Interfaces;
using TicketNotifier.Services.Implementations;
using TicketNotifier.Services.Interfaces;

namespace TicketNotifier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddSwaggerGen();
            
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());
            
            // Add the processing server as IHostedService
            services.AddHangfireServer();

            // Add framework services.
            services.AddMvc();
            
            services.AddCouchbase(options =>
            {
                options.ConnectionString = "couchbase://localhost";
                options.UserName = "mfk";
                options.Password = "123456";
                options.AddLinq();
            });
            services.AddCouchbaseBucket<ITicketBucketProvider>("ticket");

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddSingleton<ICheckEvent, CheckEvent>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime, 
            IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHangfireDashboard();
            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            recurringJobManager.AddOrUpdate("checkEvent", 
                () => serviceProvider.GetService<ICheckEvent>().Run(), "* * * * *");
            
            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket Notifier API");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            applicationLifetime.ApplicationStopped.Register(async () =>
                {
                    await app.ApplicationServices.GetRequiredService<ICouchbaseLifetimeService>().CloseAsync()
                        .ConfigureAwait(false);
                }
            );
        }
    }
}
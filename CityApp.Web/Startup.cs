using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CityApp.Data;
using Microsoft.EntityFrameworkCore;
using CityApp.Data.Seeder;
using CityApp.Data.Models;
using CityApp.Web.Middleware;
using Serilog;
using System.IO;
using CityApp.Common.Logging;
using Scrutor;
using CityApp.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using CityApp.Common.Models;
using Microsoft.AspNetCore.Http.Extensions;
using CityApp.Common.Caching;
using CityApp.Web.Infrastructure;
using NonFactors.Mvc.Grid;
using CityApp.Web.Services;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using CityApp.Common.Extensions;
using Microsoft.Net.Http.Headers;

namespace CityApp.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Serilog is our application logger. Default to Verbose. If we need to control this dynamically at some point
            //   in the future, we can: https://nblumhardt.com/2014/10/dynamically-changing-the-serilog-level/
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.With<UtcTimestampEnricher>()
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "Logs", "log-{Date}.txt"), outputTemplate: "{UtcTimestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] [{SourceContext:l}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Current Account Context. We load the appropriate connection string based on the account name in the 
            //   route data. Note that there can be other Account Contexts created, such as when moving acquisitions
            //   between partitions.
            services.AddTransient<AccountDbContextFactory>();
            services.AddScoped(provider => provider.GetService<AccountDbContextFactory>().CreateAccountContext());


            services.AddCloudscribePagination();
            //var csvFormatterOptions = new CsvFormatterOptions();

            //// Add framework services.

            //services.AddMvc(options =>
            //{              
            //    options.OutputFormatters.Add(new CsvOutputFormatter(csvFormatterOptions));
            //    options.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValue.Parse("text/csv"));
            //});

           

            services.AddMvc(opt =>
            {
                opt.AddFlagsEnumModelBinderProvider();
            });

            services.AddMvcGrid();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });



            // Common Context. There is only ever one of these per request.
            services.AddDbContext<CommonContext>(options =>
                options.UseSqlServer(Configuration["Data:Common:ConnectionString"]));


            //Seeder
            services.AddTransient<CommonSeeder>();

            // Configure AutoMapper and assert that our configuration is valid.
            services.AddAutoMapper();
            Mapper.Configuration.AssertConfigurationIsValid();

            //Add AppSettings
            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);


            // Services defined in the web project.
            services.Scan(scan => scan
                .FromAssemblyOf<ICustomWebService>()
                .AddClasses(classes => classes.AssignableTo<ICustomWebService>())
                .AsSelf()
                .WithScopedLifetime());

            // Services defined in the CityApp.Services project. Note that not all services in that project
            //   implement this interface (e.g., services that require singleton scope).
            services.Scan(scan => scan
                .FromAssemblyOf<ICustomService>()
                .AddClasses(classes => classes.AssignableTo<ICustomService>())
                .AsSelf()
                .WithScopedLifetime());




            // Required to access our Configuration from other classes.
            services.AddSingleton<IConfiguration>(Configuration);

            // Tells you about the current deployment environment (local vs. Azure).
            services.AddSingleton<IDeploymentEnvironment, DeploymentEnvironment>();

            // Redis. This must be a singleton; otherwise, we'd construct a ConnectionMultiplexer on each request, which would
            //   kill performance. Also, we're not using the IDistributedCache interface because we want full access to all of 
            //   redis's awesome capabilities.
            services.AddSingleton<RedisCache>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, CommonSeeder seeder)
        {
            seeder.Configuration = this.Configuration;

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            ForceHttps(app);

            if (env.IsDevelopment() || true)
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Security
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = "AU",
                CookieHttpOnly = true,
                CookiePath = "/",
                CookieSecure = CookieSecurePolicy.SameAsRequest,
                AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                LoginPath = new PathString($"/User/Login/"),
                AccessDeniedPath = new PathString($"/User/Login/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                ExpireTimeSpan = TimeSpan.FromHours(9),
                SlidingExpiration = true
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Accounts}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "account",
                    template: "{accountNum:long}/{controller=home}/{action=index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Accounts}/{action=Index}/{id?}");
            });

            // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    // Create the Common database schema.
                    serviceScope.ServiceProvider
                        .GetService<CommonContext>()
                        .Database
                        .Migrate();

                    // Seed the data for the Common database.
                    seeder.SeedEverythingForCommon();

                    // Create the schema for the Account databases.
                    seeder.ApplyAccountMigrations();
                }
            }
            catch (Exception ex)
            {
                Log.ForContext<Startup>().Error(ex, $"An unhandled exception occurred while trying to migrate and seed databases");
                throw;
            }
        }


        private void ForceHttps(IApplicationBuilder app)
        {
            var requireHttps = Configuration.GetValue<bool?>("AppSettings:RequireHttps") ?? false;

            if (requireHttps)
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.IsHttps)
                    {
                        await next();
                    }
                    else
                    {
                        // This is an HTTP request. Redirect to HTTPS.
                        var url = context.Request.GetEncodedUrl();
                        var httpsUrl = url.Replace("http", "https");
                        context.Response.Redirect(httpsUrl, permanent: true);
                    }
                });
            }
        }

    }
}

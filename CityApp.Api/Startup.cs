using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using CityApp.Common.Logging;
using AutoMapper;
using CityApp.Common.Models;
using CityApp.Common.Caching;
using Scrutor;
using CityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using CityApp.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CityApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Extensions;
using CityApp.Api.Middleware;
using CityApp.Api.Services;

namespace CityApp.Api
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
        public SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("pj%67=jw0w+skvc%3vzt#=!%+-qv_+6a_&k=0yaz!4$9c5zm9^"));

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Current Account Context. We load the appropriate connection string based on the account name in the 
            //   route data. Note that there can be other Account Contexts created, such as when moving acquisitions
            //   between partitions.
            services.AddTransient<AccountDbContextFactory>();
            services.AddScoped(provider => provider.GetService<AccountDbContextFactory>().CreateAccountContext());


            // Add framework services.
            // Make authentication compulsory across the board (i.e. shut
            // down EVERYTHING unless explicitly opened up).
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });


            // Use policy auth.
            services.AddAuthorization(options =>
            {
                //options.AddPolicy("CECUser",
                //                  policy => policy.RequireClaim("ValidAccount", "True"));
            });

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddApiVersioning();

            // Common Context. There is only ever one of these per request.
            services.AddDbContext<CommonContext>(options =>
                options.UseSqlServer(Configuration["Data:Common:ConnectionString"]));

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


            // Redis. This must be a singleton; otherwise, we'd construct a ConnectionMultiplexer on each request, which would
            //   kill performance. Also, we're not using the IDistributedCache interface because we want full access to all of 
            //   redis's awesome capabilities.
            services.AddSingleton<RedisCache>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
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


            //https://goblincoding.com/2016/07/07/issuing-and-authenticating-jwt-tokens-in-asp-net-core-webapi-part-ii/
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });

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

using System;
using HealthChecks.UI.Client;
using System.IO;
using System.Reflection;
using LhDev.DemoOne.Data;
using LhDev.DemoOne.Exceptions;
using LhDev.DemoOne.HealthChecks;
using LhDev.DemoOne.Middleware;
using LhDev.DemoOne.Services;
using LhDev.DemoOne.SettingsModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace LhDev.DemoOne;

public class Program
{
    public const string CookieSession = "SessionId";
    public const string CookieLoginError = "LoginError";
    public const string ListEditError = "ListEditError";

    public static JwtSettings JwtSettings { get; private set; } = null!;

    public static int Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            ReadSettings(builder);
            DbManager.InitDatabase();
            AddServices(builder.Services, builder.Configuration);
            SwaggerGenerator(builder.Services);
            AddHealthChecks(builder);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.EnableTryItOutByDefault();
                    options.DisplayRequestDuration();
                    options.InjectStylesheet("/css/swagger-dark.css");
                });
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            MapHealthChecks(app);
            ConfigApp(app);

            app.Run();
        }
        catch (DemoOneException ex)
        {
            Console.WriteLine(ex);

            return ex.ExitCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was an unexpected exception reported.\n{ex}");

            return 100;
        }

        return 0;
    }

    private static void ReadSettings(WebApplicationBuilder builder)
    {
        JwtSettings = (JwtSettings)(builder.Configuration.GetSection("Jwt").Get(typeof(JwtSettings)) ??
                                    throw DemoOneException.NoJwtSettings());

    }

    private static void AddServices(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(DbManager.ConnStr));

        //services.AddCors();
        services.AddControllersWithViews();

        // // Set up dependency injection for services required
        services.AddScoped<IDbUserService, DbUserService>();
        services.AddScoped<IDbCustomerService, DbCustomerService>();
        services.AddScoped<IJwtService, JwtService>(_ => new JwtService(JwtSettings));
    }

    private static void SwaggerGenerator(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo One", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    public static void AddHealthChecks(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck<RandomTestHealthCheck>("RandomTestHealthCheck", tags: ["test"])
            .AddCheck<DatabaseHealthCheck>("DatabaseHealthCheck", tags: ["db"]);
    }

    public static void MapHealthChecks(WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/test", new HealthCheckOptions
        {
            Predicate = reg => reg.Tags.Contains("test"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });
        app.MapHealthChecks("/health/db", new HealthCheckOptions
        {
            Predicate = reg => reg.Tags.Contains("db"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });
    }

    public static void ConfigApp(WebApplication app)
    {
        app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        // Use custom JWT middleware to ensure validation on controllers marked with JwtAuthoriseAttribute.
        app.UseMiddleware<JwtMiddleware>();

        // Use custom middle to catch exceptions and produce a standard response.
        //app.UseMiddleware<ExceptionMiddleware>();

        app.UseStaticFiles();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}





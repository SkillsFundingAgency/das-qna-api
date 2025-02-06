using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace SFA.DAS.QnA.Api
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            LogManager.Setup().LoadConfigurationFromFile("nlog.config"); 
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                logger.Info("Starting up host");

                var builder = WebApplication.CreateBuilder(args);

                // Configure NLog
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Host.UseNLog();

                // Initialize Startup for services and middleware
                var startup = new Startup(builder.Configuration, builder.Environment);
                startup.ConfigureServices(builder.Services);

                var app = builder.Build();
                startup.Configure(app, app.Environment);

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
    }
}
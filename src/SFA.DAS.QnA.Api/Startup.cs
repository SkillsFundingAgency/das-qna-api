using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Application;
using SFA.DAS.QnA.Configuration.Config;
using SFA.DAS.QnA.Configuration.Infrastructure;

namespace SFA.DAS.QnA.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddAzureTableStorageConfiguration(
                    configuration["ConfigurationStorageConnectionString"],
                    configuration["ConfigNames"],
                    configuration["Environment"],
                    configuration["Version"]
                ).Build();

            Configuration = config;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<QnAConfig>(Configuration.GetSection("QnA"));
            services.Configure<AuthenticationConfig>(Configuration.GetSection("ApiAuthentication"));
            var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetService<IOptions<QnAConfig>>();

            if (!_hostingEnvironment.IsDevelopment())
            {
                var azureActiveDirectoryConfiguration =
                    serviceProvider.GetService<IOptions<AuthenticationConfig>>();
                services.AddAuthorization(o =>
                {
                    o.AddPolicy("default", policy => { policy.RequireAuthenticatedUser(); });
                });
                services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                    .AddJwtBearer(auth =>
                    {
                        auth.Authority =
                            $"https://login.microsoftonline.com/{azureActiveDirectoryConfiguration.Value.TenantId}";
                        auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidAudiences = new List<string>
                            {
                                azureActiveDirectoryConfiguration.Value.Audience,
                                azureActiveDirectoryConfiguration.Value.ClientId.ToString()
                            }
                        };
                    });
            }
            
            services.AddMediatR(typeof(Class1).Assembly);

            services.AddDbContext<QnaDataContext>(options => options.UseSqlServer(config.Value.SqlConnectionstring));

            services.AddEntityFrameworkSqlServer();

            services.AddMvc(setup => {
                if (!_hostingEnvironment.IsDevelopment())
                {
                    setup.Filters.Add(new AuthorizeFilter("default"));
                }
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"); });
        }
    }
}
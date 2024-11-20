using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.QnA.Api.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services, IWebHostEnvironment environment)
        {
            var isDevelopment = environment.IsDevelopment();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    if (isDevelopment)
                    {
                        policy.RequireAssertion(_ => true);
                    }
                    else
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Default");
                    }
                });

                options.DefaultPolicy = options.GetPolicy("Default");
            });

            if (isDevelopment)
            {
                services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();
            }

            return services;
        }
    }
}

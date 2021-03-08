using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace SFA.DAS.QnA.Api.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services, IHostingEnvironment environment)
        {
            var isDevelopment = environment.IsDevelopment();

            services.AddAuthorization(x =>
            {
                x.AddPolicy("default", policy =>
                {
                    if (isDevelopment)
                    {
                        policy.AllowAnonymousUser();
                    }
                    else
                    {
                        policy.RequireAuthenticatedUser();
                    }
                });

                x.DefaultPolicy = x.GetPolicy("default");
            });

            if (isDevelopment)
            {
                services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();
            }

            return services;
        }
    }
}

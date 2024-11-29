using System.Reflection;
using Application.FeatureBookReview.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ConfigurationExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(ApplicationLibrary).Assembly));
        
        services.AddScoped<IBookReviewService, BookReviewService>();
    }
}
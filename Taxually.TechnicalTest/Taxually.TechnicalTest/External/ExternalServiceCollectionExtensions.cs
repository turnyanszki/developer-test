namespace Taxually.TechnicalTest.External
{
    public static class ExternalServiceCollectionExtensions
    {
        public static IServiceCollection AddExternalConnectors(this IServiceCollection services)
        {
            services.AddScoped<ITaxuallyHttpClient, TaxuallyHttpClient>();
            services.AddScoped<ITaxuallyQueueClient, TaxuallyQueueClient>();
            return services;
        }
    }
}

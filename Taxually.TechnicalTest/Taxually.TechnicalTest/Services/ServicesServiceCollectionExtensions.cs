namespace Taxually.TechnicalTest.Services
{
    public static class ServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddTaxuallyServices(this IServiceCollection services)
        {
            services.AddScoped<IVatRegistrationService,VatRegistrationService>();
            return services;
        }
            
    }
}

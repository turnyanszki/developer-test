using FluentValidation;
using Taxually.TechnicalTest.Contracts.Requests;

namespace Taxually.TechnicalTest.Validators
{
    public static class ValidatorsServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services) 
        {
            services.AddScoped<IValidator<VatRegistrationRequest>, VatRegistrationRequestValidator>();
            services.AddScoped<ITaxuallyTechTestValidator, TaxuallyTechTestValidator>();
            return services;
        }
    }
}

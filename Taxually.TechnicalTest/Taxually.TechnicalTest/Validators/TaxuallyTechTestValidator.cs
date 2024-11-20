using FluentValidation;
using Taxually.TechnicalTest.Contracts.Responses;

namespace Taxually.TechnicalTest.Validators
{
    public class TaxuallyTechTestValidator : ITaxuallyTechTestValidator
    {
        private readonly IServiceProvider _serviceProvider;

        public TaxuallyTechTestValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public (bool status, ErrorResponse errorResponse) Validate<T>(T contract, string correlationId)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<T>>();
            var validationResult = validator.Validate(contract);
            if (validationResult.IsValid) return (true, null);
            var errorResponse = new ErrorResponse
            {
                CorrelationId = correlationId,
                Errors = validationResult.Errors.Select(x => new Error
                {
                    ErrorCode = x.ErrorCode,
                    ErrorMessage = x.ErrorMessage
                }
                ).ToList()
            };
            return (false, errorResponse);
        }
    }
}

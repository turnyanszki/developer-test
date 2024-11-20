using FluentValidation;
using Taxually.TechnicalTest.Contracts.Requests;

namespace Taxually.TechnicalTest.Validators
{
    public class VatRegistrationRequestValidator : AbstractValidator<VatRegistrationRequest>
    {
        public VatRegistrationRequestValidator()
        {
            RuleFor(vatRegistrationRequest => vatRegistrationRequest.CorrelationId).NotEmpty();
            RuleFor(vatRegistrationRequest => vatRegistrationRequest.CompanyName).NotEmpty();
            RuleFor(vatRegistrationRequest => vatRegistrationRequest.Country).IsInEnum();
        }
    }
}

using FluentValidation;
using Taxually.TechnicalTest.Contracts.Responses;

namespace Taxually.TechnicalTest.Validators
{
    public interface ITaxuallyTechTestValidator
    {
        (bool status, ErrorResponse errorResponse) Validate<T>(T contract, string correlationId);
    }
}

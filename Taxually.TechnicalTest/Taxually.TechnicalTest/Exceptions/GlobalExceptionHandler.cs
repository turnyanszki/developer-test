using Microsoft.AspNetCore.Diagnostics;
using Taxually.TechnicalTest.Contracts.Responses;
using Taxually.TechnicalTest.Services;

namespace Taxually.TechnicalTest.Exceptions
{
    //Exception handling is just an example of how to do it, not fully implemented
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var correlationId = CorrelationIdProvider.GetCorrelationId();
            if (exception is TaxuallyExternalException externalException)
            {
                var errorResponse = new ErrorResponse
                {
                    CorrelationId = correlationId,
                    Errors = new List<Error> { { new Error { ErrorCode = "1.2.3", ErrorMessage = $"Error happened during external call: {externalException.ExternalService}" } } }
                };
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsJsonAsync(errorResponse);
                return true;
            }

            if (exception is TaxuallyInternalException internalException)
            {
                var errorResponse = new ErrorResponse
                {
                    CorrelationId = correlationId,
                    Errors = new List<Error> { { new Error { ErrorCode = "1.2.3", ErrorMessage = internalException.Message } } }
                };
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsJsonAsync(errorResponse);
                return true;
            }

            return false;
        }
    }
}

using System.Text;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Taxually.TechnicalTest.Contracts.Requests;
using Taxually.TechnicalTest.Contracts.Responses;
using Taxually.TechnicalTest.POCO;
using Taxually.TechnicalTest.Services;
using Taxually.TechnicalTest.Validators;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taxually.TechnicalTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VatRegistrationController : ControllerBase
    {
        private readonly ITaxuallyTechTestValidator _taxuallyTechTestValidator;
        private readonly IVatRegistrationService _vatRegistrationService;

        public VatRegistrationController(ITaxuallyTechTestValidator taxuallyTechTestValidator, IVatRegistrationService vatRegistrationService)
        {
            _taxuallyTechTestValidator = taxuallyTechTestValidator;
            _vatRegistrationService = vatRegistrationService;
        }
        /// <summary>
        /// Registers a company for a VAT number in a given country
        /// </summary>
        [HttpPost]
        [ProducesResponseType<VatRegistrationResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] VatRegistrationRequest request)
        {
            var validationResult = _taxuallyTechTestValidator.Validate(request, request.CorrelationId);
            if (!validationResult.status)
                return BadRequest(validationResult.errorResponse);

            await _vatRegistrationService.RegistrateVat(VatRegistration.Create(request));

            return Ok(new VatRegistrationResponse { CorrelationId = request.CorrelationId });
        }
    }

}

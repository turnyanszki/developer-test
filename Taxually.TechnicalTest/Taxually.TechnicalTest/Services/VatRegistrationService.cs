using System.Text;
using System.Xml.Serialization;
using Taxually.TechnicalTest.Exceptions;
using Taxually.TechnicalTest.External;
using Taxually.TechnicalTest.POCO;
using static Taxually.TechnicalTest.External.TaxuallyQueueClient;

namespace Taxually.TechnicalTest.Services
{
    //Logging could be added everywhere, I added one example 

    public class VatRegistrationService : IVatRegistrationService
    {
        private readonly ILogger<VatRegistrationService> _logger;
        private readonly ITaxuallyQueueClient _taxuallyQueueClient;
        private readonly ITaxuallyHttpClient _taxuallyHttpClient;

        public VatRegistrationService(ILogger<VatRegistrationService> logger, ITaxuallyQueueClient taxuallyQueueClient, ITaxuallyHttpClient taxuallyHttpClient)
        {
            _logger = logger;
            _taxuallyQueueClient = taxuallyQueueClient;
            _taxuallyHttpClient = taxuallyHttpClient;
        }
        public async Task RegistrateVat(VatRegistration vatRegistration)
        {
            switch (vatRegistration.Country)
            {
                case Country.GB:
                    await RegistrateVatGB(vatRegistration);
                    break;
                case Country.FR:
                    await RegistrateVatFR(vatRegistration);
                    break;
                case Country.DE:
                    await RegistrateVatDE(vatRegistration);
                    break;
                default:
                    //We should never reach this due to validation
                    throw new Exception("Country not supported");
            }
        }

        internal async Task RegistrateVatGB(VatRegistration vatRegistration)
        {
            try
            {
                // UK has an API to register for a VAT number
                _logger.LogInformation("Starting processing GB vat registration");
                //MISSING: Map VatRegistration to the appropriate external contract here
                await _taxuallyHttpClient.PostAsync("https://api.uktax.gov.uk", vatRegistration);
            }
            catch (Exception ex)
            {
                throw new TaxuallyExternalException("Problem with calling UK tax gov with vat request", ex, nameof(TaxuallyHttpClient));
            }
        }

        internal async Task RegistrateVatFR(VatRegistration vatRegistration)
        {
            // France requires an excel spreadsheet to be uploaded to register for a VAT number
            try
            {
                //MISSING: Map VatRegistration to the appropriate external contract here

                byte[] csv = GenerateFRCsv(vatRegistration);
                // Queue file to be processed
                await _taxuallyQueueClient.EnqueueAsync("vat-registration-csv", csv);
            }
            catch (QueueException ex)
            {
                throw new TaxuallyExternalException("Problem with enqueueing french vat request", ex, nameof(TaxuallyQueueClient));
            }

        }

        internal async Task RegistrateVatDE(VatRegistration vatRegistration)
        {
            // Germany requires an XML document to be uploaded to register for a VAT number
            try
            {
                //MISSING: Map VatRegistration to the appropriate external contract here
                var xml = GenerateDEXml(vatRegistration);
                // Queue xml doc to be processed
                await _taxuallyQueueClient.EnqueueAsync("vat-registration-xml", xml);
            }
            catch (QueueException ex)
            {
                throw new TaxuallyExternalException("Problem with enqueueing german vat request", ex, nameof(TaxuallyQueueClient));
            }
        }
        
        //Could be in some CSV generator service, for now as this is the only usage of csv it's fine here
        internal static byte[] GenerateFRCsv(VatRegistration vatRegistration)
        {
            try
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("CompanyName,CompanyId");
                csvBuilder.AppendLine($"{vatRegistration.CompanyName}{vatRegistration.CompanyId}");
                var csv = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                return csv;
            }
            catch (Exception ex)
            {
                throw new TaxuallyInternalException("Problem with french vat request csv generation", ex);
            }
        }
        //Could be in some XML generator service, for now as this is the only usage of csv it's fine here
        internal static string GenerateDEXml(VatRegistration vatRegistration)
        {
            try
            {
                using (var stringwriter = new StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(VatRegistration));
                    serializer.Serialize(stringwriter, vatRegistration);
                    var xml = stringwriter.ToString();
                    return xml;
                }
            }
            catch (Exception ex)
            {
                throw new TaxuallyInternalException("Problem with german vat request xml generation", ex);
            }
        }
    }
}
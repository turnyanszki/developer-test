using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text;
using Taxually.TechnicalTest.Exceptions;
using Taxually.TechnicalTest.External;
using Taxually.TechnicalTest.POCO;
using Taxually.TechnicalTest.Services;
using static Taxually.TechnicalTest.External.TaxuallyQueueClient;

namespace Taxually.TechnicalTest.Tests
{
    public class VatRegistrationServiceTests
    {
        private readonly ILogger<VatRegistrationService> _mockLogger;
        private readonly IVatRegistrationService _service;
        private readonly ITaxuallyHttpClient _mockHttpClient;
        private readonly ITaxuallyQueueClient _mockQueueClient;

        public VatRegistrationServiceTests()
        {
            _mockLogger = Substitute.For<ILogger<VatRegistrationService>>();
            _mockHttpClient = Substitute.For<ITaxuallyHttpClient>();
            _mockQueueClient = Substitute.For<ITaxuallyQueueClient>();
            _service = new VatRegistrationService(_mockLogger, _mockQueueClient, _mockHttpClient);
        }

        [Fact]
        public async Task RegistrateVat_ShouldCallGBRegistration_WhenCountryIsGB()
        {
            // Arrange
            var vatRegistration = new VatRegistration
            {
                Country = Country.GB,
                CompanyName = "Test Company",
                CompanyId = "123456"
            };

            // Act
            await _service.RegistrateVat(vatRegistration);

            // Assert
            _mockLogger.Received(1).LogInformation("Starting processing GB vat registration");
            await _mockHttpClient.Received(1).PostAsync("https://api.uktax.gov.uk", vatRegistration);
        }

        [Fact]
        public async Task RegistrateVat_ShouldCallFRRegistration_WhenCountryIsFR()
        {
            // Arrange
            var vatRegistration = new VatRegistration
            {
                Country = Country.FR,
                CompanyName = "Test Company",
                CompanyId = "123456"
            };

            byte[] expectedCsv = Encoding.UTF8.GetBytes("CompanyName,CompanyId\nTest Company123456\n");

            // Act
            await _service.RegistrateVat(vatRegistration);

            // Assert
            await _mockQueueClient.Received(1).EnqueueAsync("vat-registration-csv", Arg.Any<byte[]>());
        }

        [Fact]
        public async Task RegistrateVat_ShouldCallDERegistration_WhenCountryIsDE()
        {
            // Arrange
            var vatRegistration = new VatRegistration
            {
                Country = Country.DE,
                CompanyName = "Test Company",
                CompanyId = "123456"
            };

            // Simulate XML generation
            string expectedXml = "<VatRegistration><CompanyName>Test Company</CompanyName><CompanyId>123456</CompanyId></VatRegistration>";

            // Act
            await _service.RegistrateVat(vatRegistration);

            // Assert
            await _mockQueueClient.Received(1).EnqueueAsync("vat-registration-xml", Arg.Is<string>(xml => xml.Contains("Test Company")));
        }

        [Fact]
        public async Task RegistrateVat_ShouldThrowException_WhenCountryNotSupported()
        {
            // Arrange
            var vatRegistration = new VatRegistration
            {
                Country = (Country)999, // Invalid country
                CompanyName = "Test Company",
                CompanyId = "123456"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.RegistrateVat(vatRegistration));
        }

        [Fact]
        public async Task RegistrateVatGB_ShouldThrowExternalException_OnHttpClientFailure()
        {
            // Arrange
            var vatRegistration = new VatRegistration
            {
                Country = Country.GB,
                CompanyName = "Test Company",
                CompanyId = "123456"
            };
            _mockHttpClient.When(x => x.PostAsync(Arg.Any<string>(), Arg.Any<object>())).Throw(new Exception("HTTP Error"));

            // Act & Assert
            await Assert.ThrowsAsync<TaxuallyExternalException>(() => _service.RegistrateVat(vatRegistration));
        }

        [Fact]
        public async Task RegistrateVatFR_ShouldThrowExternalException_OnQueueFailure()
        {
            // Arrange
            var vatRegistration = new VatRegistration
            {
                Country = Country.FR,
                CompanyName = "Test Company",
                CompanyId = "123456"
            };

            _mockQueueClient.When(x => x.EnqueueAsync(Arg.Any<string>(), Arg.Any<byte[]>())).Throw(new QueueException());

            // Act & Assert
            await Assert.ThrowsAsync<TaxuallyExternalException>(() => _service.RegistrateVat(vatRegistration));
        }

        [Fact]
        public async Task RegistrateVatDE_ShouldThrowExternalException_OnQueueFailure()
        {
            // Arrange
            var vatRegistration = new VatRegistration
            {
                Country = Country.DE,
                CompanyName = "Test Company",
                CompanyId = "123456"
            };

            _mockQueueClient.When(x => x.EnqueueAsync(Arg.Any<string>(), Arg.Any<string>())).Throw(new QueueException());

            // Act & Assert
            await Assert.ThrowsAsync<TaxuallyExternalException>(() => _service.RegistrateVat(vatRegistration));
        }


        [Fact]
        public void GenerateDEXml_ShouldThrowInternalException_OnFailure()
        {
            // Arrange
            var vatRegistration = new VatRegistration
            {
                CompanyName = "Test Company",
                CompanyId = "123456",
                Country = 0 // Invalid value
            };

            // Act & Assert
            Assert.Throws<TaxuallyInternalException>(() => VatRegistrationService.GenerateDEXml(vatRegistration));
        }
    }

}
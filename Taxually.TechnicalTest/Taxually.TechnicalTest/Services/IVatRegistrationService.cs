using Taxually.TechnicalTest.POCO;

namespace Taxually.TechnicalTest.Services
{
    public interface IVatRegistrationService
    {
        Task RegistrateVat(VatRegistration vatRegistration);
    }
}

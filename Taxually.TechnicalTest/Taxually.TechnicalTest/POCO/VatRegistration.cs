using Taxually.TechnicalTest.Contracts.Requests;

namespace Taxually.TechnicalTest.POCO
{
    public class VatRegistration
    {
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public Country Country { get; set; }

        public static VatRegistration Create(VatRegistrationRequest vatRegistrationRequest)
        {
            return new VatRegistration
            {
                CompanyId = vatRegistrationRequest.CompanyId,
                CompanyName = vatRegistrationRequest.CompanyName,
                Country = vatRegistrationRequest.Country switch
                {
                    Contracts.Country.FR => Country.FR,
                    Contracts.Country.DE => Country.DE,
                    Contracts.Country.GB => Country.GB,
                    _ => 0
                }
            };
        }
    }
}

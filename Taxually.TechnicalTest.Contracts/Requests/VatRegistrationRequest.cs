namespace Taxually.TechnicalTest.Contracts.Requests;
public class VatRegistrationRequest : BaseRequest
{
    public string CompanyName { get; set; }
    public string CompanyId { get; set; }
    public Country Country { get; set; }
}


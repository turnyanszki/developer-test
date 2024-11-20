namespace Taxually.TechnicalTest.Contracts.Responses
{
    public class Error
    {
        public string ErrorCode { get; set; }
        //Error message in english in case of missing translation on FE
        public string ErrorMessage { get; set; }
    }
}
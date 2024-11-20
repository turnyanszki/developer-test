namespace Taxually.TechnicalTest.Exceptions
{
    public class TaxuallyExternalException : Exception
    {
        public TaxuallyExternalException(string message, Exception inner, string externalService): base(message, inner)
        {
            ExternalService = externalService;
        }
        public string ExternalService { get; set; }
    }
}

namespace Taxually.TechnicalTest.Exceptions
{
    public class TaxuallyInternalException : Exception
    {
        public TaxuallyInternalException(string message, Exception inner): base(message, inner) { }
    }
}

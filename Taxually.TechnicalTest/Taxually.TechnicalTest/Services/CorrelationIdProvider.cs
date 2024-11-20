namespace Taxually.TechnicalTest.Services
{
    //CorrelationId should come from a header and put into an http scoped provider within an action filter to be able to retrieve it in any service, I won't implement it within this task scope
    public static class CorrelationIdProvider
    {
        public static string GetCorrelationId() => Guid.NewGuid().ToString();
    }
}

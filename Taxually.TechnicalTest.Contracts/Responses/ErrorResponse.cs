namespace Taxually.TechnicalTest.Contracts.Responses
{
    public class ErrorResponse : BaseResponse
    {
        public List<Error> Errors { get; set; }
    }
}

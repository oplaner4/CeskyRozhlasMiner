using System.Net;

namespace Microsoft.DSX.ProjectTemplate.Data.Exceptions
{
    public class NotAcceptableException : ExceptionBase
    {
        private static string DefaultMessageHeader => "Not acceptable";

        public override HttpStatusCode StatusCode => HttpStatusCode.NotAcceptable;

        public NotAcceptableException(string message, string messageHeader = null)
            : base(message, messageHeader ?? DefaultMessageHeader) { }
    }
}

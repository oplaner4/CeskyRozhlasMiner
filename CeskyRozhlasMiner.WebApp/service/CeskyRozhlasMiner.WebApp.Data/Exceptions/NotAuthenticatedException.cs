using System.Net;

namespace Microsoft.DSX.ProjectTemplate.Data.Exceptions
{
    public class NotAuthenticatedException : ExceptionBase
    {
        private static string DefaultMessageHeader => "Not authenticated";

        public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;

        public NotAuthenticatedException(string message, string messageHeader = null)
            : base(message, messageHeader ?? DefaultMessageHeader) { }
    }
}

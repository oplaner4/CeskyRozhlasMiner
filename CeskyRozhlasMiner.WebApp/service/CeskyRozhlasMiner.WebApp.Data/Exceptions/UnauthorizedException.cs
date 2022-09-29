using System.Net;

namespace Microsoft.DSX.ProjectTemplate.Data.Exceptions
{
    public class UnauthorizedException : ExceptionBase
    {
        private static string DefaultMessageHeader => "Unauthorized";

        public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;

        public UnauthorizedException(string message, string messageHeader = null)
            : base(message, messageHeader ?? DefaultMessageHeader) { }
    }
}

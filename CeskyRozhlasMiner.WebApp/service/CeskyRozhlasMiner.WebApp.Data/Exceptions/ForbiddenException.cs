using System.Net;

namespace Microsoft.DSX.ProjectTemplate.Data.Exceptions
{
    public class ForbiddenException : ExceptionBase
    {
        private static string DefaultMessageHeader => "Forbidden";

        public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

        public ForbiddenException(string message, string messageHeader = null)
            : base(message, messageHeader ?? DefaultMessageHeader) { }
    }
}

using MediatR;
using Microsoft.DSX.ProjectTemplate.Data.Models;

namespace Microsoft.DSX.ProjectTemplate.Data.Events
{
    public class TokenGeneratedDomainEvent : INotification
    {
        public Token Token { get; }

        public TokenGeneratedDomainEvent(Token token)
        {
            Token = token;
        }
    }
}

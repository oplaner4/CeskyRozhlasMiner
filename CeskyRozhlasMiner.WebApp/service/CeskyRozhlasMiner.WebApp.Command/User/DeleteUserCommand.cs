using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.User
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public UserAuthenticateDto User { get; set; }
    }

    public class DeleteUserCommandHandler : CommandHandlerBase, IRequestHandler<DeleteUserCommand, bool>
    {
        public DeleteUserCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.User;

            var user = await Database.Users.FirstOrDefaultAsync(u => !u.Deleted && u.Email == dto.Email, cancellationToken);

            if (user == null)
            {
                throw new EntityNotFoundException($"{nameof(Data.Models.User)} cannot be found.");
            }

            var verification = new PasswordHasher<Data.Models.User>().VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException($"Invalid credentials were provided.");
            }

            if (user.Id == UserId)
            {
                await Mediator.Send(new SignOutUserCommand(), cancellationToken);
            }

            user.Deleted = true;
            Database.Users.Update(user);
            await Database.SaveChangesAsync(cancellationToken);
            return user.Deleted;
        }
    }
}

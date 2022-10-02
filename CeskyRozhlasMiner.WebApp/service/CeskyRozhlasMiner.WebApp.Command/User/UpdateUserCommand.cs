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
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public UserSetDto User { get; set; }
    }

    public class UpdateUserCommandHandler : CommandHandlerBase, IRequestHandler<UpdateUserCommand, UserDto>
    {
        public UpdateUserCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
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

            user.DisplayName = dto.DisplayName;
            user.PasswordHash = new PasswordHasher<Data.Models.User>().HashPassword(user, dto.NewPassword);
            await Database.SaveChangesAsync(cancellationToken);

            return Mapper.Map<UserDto>(user);
        }
    }
}

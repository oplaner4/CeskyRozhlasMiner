using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Events;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using RadiozurnalMiner.Lib.Playlist;
using System.Security.Cryptography;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Group
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public UserCreateDto User { get; set; }
    }

    public class CreateUserCommandHandler : CommandHandlerBase, IRequestHandler<CreateUserCommand, UserDto>
    {
        public CreateUserCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IAuthorizationService authorizationService)
            : base(mediator, database, mapper, authorizationService)
        {
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.User;

            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var model = new Data.Models.User()
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                PasswordSalt = Convert.ToBase64String(salt),
                PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: dto.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8)),
            };

            Database.Users.Add(model);

            await Database.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new UserCreatedDomainEvent(model), cancellationToken);

            return Mapper.Map<UserDto>(model);
        }
    }
}

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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

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
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.User;
            
            bool emailAlreadyUsed = await Database.Users.AnyAsync(e => e.Email == dto.Email, cancellationToken);

            if (emailAlreadyUsed)
            {
                throw new BadRequestException($"Email adress already used.");
            }

            var model = new Data.Models.User()
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
            };

            model.PasswordHash = new PasswordHasher<Data.Models.User>().HashPassword(model, dto.Password);

            Database.Users.Add(model);
            await Database.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new UserCreatedDomainEvent(model), cancellationToken);
            
            

            return Mapper.Map<UserDto>(model);
        }
    }
}

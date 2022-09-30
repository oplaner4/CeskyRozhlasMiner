﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Events;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.User
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public UserSetDto User { get; set; }
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

            bool emailAlreadyUsed = await Database.Users.AnyAsync(e => !e.Deleted && e.Email == dto.Email, cancellationToken);

            if (emailAlreadyUsed)
            {
                throw new BadRequestException($"Email adress already used.");
            }

            var model = new Data.Models.User()
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
            };

            model.PasswordHash = new PasswordHasher<Data.Models.User>().HashPassword(model, dto.NewPassword);

            Database.Users.Add(model);
            await Database.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new UserCreatedDomainEvent(model), cancellationToken);

            return Mapper.Map<UserDto>(model);
        }
    }
}

﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Events;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Group
{
    public class CreateGroupCommand : IRequest<GroupDto>
    {
        public GroupDto Group { get; set; }
    }

    public class CreateGroupCommandHandler : CommandHandlerBase,
        IRequestHandler<CreateGroupCommand, GroupDto>
    {
        public CreateGroupCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Group;

            bool nameAlreadyUsed = await Database.Groups.AnyAsync(e => e.Name.Trim() == dto.Name.Trim(), cancellationToken);
            if (nameAlreadyUsed)
            {
                throw new NotAcceptableException($"{nameof(dto.Name)} '{dto.Name}' already used.");
            }

            var model = new Data.Models.Group()
            {
                Name = dto.Name,
                IsActive = dto.IsActive
            };

            Database.Groups.Add(model);

            await Database.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new GroupCreatedDomainEvent(model), cancellationToken);

            return Mapper.Map<GroupDto>(model);
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Sprints;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Sprints.Commands;

public record CreateSprintCommand(Guid ProjectId, CreateSprintDto Dto) : IRequest<SprintDto>;

public class CreateSprintCommandHandler : IRequestHandler<CreateSprintCommand, SprintDto>
{
    private readonly IAppDbContext _db;

    public CreateSprintCommandHandler(IAppDbContext db) => _db = db;

    public async Task<SprintDto> Handle(CreateSprintCommand request, CancellationToken cancellationToken)
    {
        var sprint = new Sprint
        {
            Id = Guid.NewGuid(),
            Name = request.Dto.Name,
            Goal = request.Dto.Goal,
            StartDate = request.Dto.StartDate,
            EndDate = request.Dto.EndDate,
            Status = SprintStatus.Planning,
            ProjectId = request.ProjectId
        };

        _db.Sprints.Add(sprint);
        await _db.SaveChangesAsync(cancellationToken);

        return new SprintDto(sprint.Id, sprint.Name, sprint.Goal, sprint.StartDate,
            sprint.EndDate, sprint.Status, sprint.ProjectId, 0, 0);
    }
}

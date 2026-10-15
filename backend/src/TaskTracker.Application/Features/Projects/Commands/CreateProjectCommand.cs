using MediatR;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Projects.Commands;

public record CreateProjectCommand(CreateProjectDto Dto, Guid UserId) : IRequest<ProjectDto>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IAppDbContext _db;

    public CreateProjectCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Dto.Name,
            Key = request.Dto.Key.ToUpperInvariant(),
            Description = request.Dto.Description,
            OwnerId = request.UserId
        };

        _db.Projects.Add(project);

        _db.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = project.Id,
            UserId = request.UserId,
            Role = UserRole.ProjectManager
        });

        var board = new Board
        {
            Id = Guid.NewGuid(),
            Name = "Main Board",
            ProjectId = project.Id,
            Columns = new List<BoardColumn>
            {
                new() { Id = Guid.NewGuid(), Name = "К выполнению", Order = 0, MappedStatus = IssueStatus.Todo },
                new() { Id = Guid.NewGuid(), Name = "В работе", Order = 1, MappedStatus = IssueStatus.InProgress },
                new() { Id = Guid.NewGuid(), Name = "Тестирование", Order = 2, MappedStatus = IssueStatus.Testing },
                new() { Id = Guid.NewGuid(), Name = "Готово", Order = 3, MappedStatus = IssueStatus.Done }
            }
        };
        _db.Boards.Add(board);

        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectDto(project.Id, project.Name, project.Key, project.Description,
            project.OwnerId, string.Empty, project.CreatedAt, 1, 0);
    }
}

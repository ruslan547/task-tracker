using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Issues;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Features.Issues.Commands;
using TaskTracker.Application.Features.Projects.Commands;
using TaskTracker.Application.Features.Projects.Queries;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Tests;

public class ProjectTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateProject_ShouldCreateProjectWithBoard()
    {
        using var context = CreateInMemoryContext();
        var handler = new CreateProjectCommandHandler(context);

        var userId = Guid.NewGuid();
        var dto = new CreateProjectDto("Test Project", "TEST", "A test project");
        var command = new CreateProjectCommand(dto, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("Test Project", result.Name);
        Assert.Equal("TEST", result.Key);
        Assert.Equal(1, result.MemberCount);

        var board = await context.Boards.Include(b => b.Columns)
            .FirstOrDefaultAsync(b => b.ProjectId == result.Id);
        Assert.NotNull(board);
        Assert.Equal(4, board.Columns.Count);
    }

    [Fact]
    public async Task GetProjects_ShouldReturnUserProjects()
    {
        using var context = CreateInMemoryContext();
        var userId = Guid.NewGuid();

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "My Project",
            Key = "MP",
            OwnerId = userId,
            Owner = new AppUser { Id = userId, FullName = "Test User", UserName = "test@test.com", Email = "test@test.com" }
        };
        context.Projects.Add(project);
        context.ProjectMembers.Add(new ProjectMember { ProjectId = project.Id, UserId = userId, Role = UserRole.ProjectManager });
        await context.SaveChangesAsync();

        var handler = new GetProjectsQueryHandler(context);
        var result = await handler.Handle(new GetProjectsQuery(userId), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("My Project", result[0].Name);
    }

    [Fact]
    public async Task DeleteProject_ShouldRemoveProject()
    {
        using var context = CreateInMemoryContext();
        var project = new Project { Id = Guid.NewGuid(), Name = "To Delete", Key = "DEL", OwnerId = Guid.NewGuid() };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var handler = new DeleteProjectCommandHandler(context);
        await handler.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        Assert.Empty(context.Projects);
    }
}

public class IssueTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateIssue_ShouldAssignIssueKeyAndNumber()
    {
        using var context = CreateInMemoryContext();
        var userId = Guid.NewGuid();
        var project = new Project { Id = Guid.NewGuid(), Name = "Proj", Key = "PRJ", OwnerId = userId };
        var user = new AppUser { Id = userId, FullName = "Test", UserName = "t@t.com", Email = "t@t.com" };
        context.Projects.Add(project);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateIssueCommandHandler(context);
        var dto = new CreateIssueDto("Bug fix", null, IssueType.Bug, IssuePriority.High, null, project.Id, null, null, null);
        var result = await handler.Handle(new CreateIssueCommand(dto, userId), CancellationToken.None);

        Assert.Equal("PRJ-1", result.IssueKey);
        Assert.Equal(1, result.IssueNumber);
        Assert.Equal(IssueType.Bug, result.Type);
    }

    [Fact]
    public async Task MoveIssue_ShouldUpdateStatus()
    {
        using var context = CreateInMemoryContext();
        var issue = new Issue
        {
            Id = Guid.NewGuid(), Title = "Task", IssueKey = "T-1", IssueNumber = 1,
            Status = IssueStatus.Todo, ProjectId = Guid.NewGuid(), ReporterId = Guid.NewGuid()
        };
        context.Issues.Add(issue);
        await context.SaveChangesAsync();

        var handler = new MoveIssueCommandHandler(context);
        await handler.Handle(new MoveIssueCommand(issue.Id, IssueStatus.InProgress), CancellationToken.None);

        var updated = await context.Issues.FindAsync(issue.Id);
        Assert.Equal(IssueStatus.InProgress, updated!.Status);
    }

    [Fact]
    public async Task MoveIssue_FullCycle_TodoToDone()
    {
        using var context = CreateInMemoryContext();
        var issue = new Issue
        {
            Id = Guid.NewGuid(), Title = "Full Cycle", IssueKey = "FC-1", IssueNumber = 1,
            Status = IssueStatus.Todo, ProjectId = Guid.NewGuid(), ReporterId = Guid.NewGuid()
        };
        context.Issues.Add(issue);
        await context.SaveChangesAsync();

        var handler = new MoveIssueCommandHandler(context);

        await handler.Handle(new MoveIssueCommand(issue.Id, IssueStatus.InProgress), CancellationToken.None);
        await handler.Handle(new MoveIssueCommand(issue.Id, IssueStatus.Testing), CancellationToken.None);
        await handler.Handle(new MoveIssueCommand(issue.Id, IssueStatus.Done), CancellationToken.None);

        var final = await context.Issues.FindAsync(issue.Id);
        Assert.Equal(IssueStatus.Done, final!.Status);
    }

    [Fact]
    public async Task DeleteIssue_ShouldRemoveIssue()
    {
        using var context = CreateInMemoryContext();
        var issue = new Issue
        {
            Id = Guid.NewGuid(), Title = "To Delete", IssueKey = "D-1", IssueNumber = 1,
            ProjectId = Guid.NewGuid(), ReporterId = Guid.NewGuid()
        };
        context.Issues.Add(issue);
        await context.SaveChangesAsync();

        var handler = new DeleteIssueCommandHandler(context);
        await handler.Handle(new DeleteIssueCommand(issue.Id), CancellationToken.None);

        Assert.Empty(context.Issues);
    }
}

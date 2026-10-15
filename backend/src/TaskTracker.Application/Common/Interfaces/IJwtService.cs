using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser user);
}

using Microsoft.Extensions.Logging;
using TaskTracker.Application.Common.Interfaces;

namespace TaskTracker.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Email to {To}: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}

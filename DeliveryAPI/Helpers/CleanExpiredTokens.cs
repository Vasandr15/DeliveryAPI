using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Helpers;

public class CleanExpiredTokens : BackgroundService
{
    private readonly TimeSpan _cleanMinutes;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CleanExpiredTokens(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _cleanMinutes = TimeSpan.FromMinutes(configuration.GetValue<int>("TokenCleanTime"));
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanExpiredTokensAsync(stoppingToken);
            await Task.Delay(_cleanMinutes, stoppingToken);
        }
    }

    private async Task CleanExpiredTokensAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContexts>();
        
        var expiredTokens = await context.Tokens
            .Where(token => token.ExpiredDate <= DateTime.UtcNow)
            .ToListAsync(cancellationToken: stoppingToken);

        foreach (var token in expiredTokens)
        {
            context.Tokens.Remove(token);
        }
        await context.SaveChangesAsync(stoppingToken);
    }
}

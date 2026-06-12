using ECommerce.Application.Features.Maintenance.AutoCompleteOrders;
using ECommerce.Application.Features.Maintenance.ReleaseExpiredReservations;
using MediatR;

namespace ECommerce.Api.BackgroundServices;

/// <summary>
/// Periodic maintenance (the doc's Hangfire jobs, here as a hosted service): releases expired
/// stock reservations (BR-03) every minute and auto-completes delivered orders (BR-19).
/// </summary>
public class MaintenanceBackgroundService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MaintenanceBackgroundService> _logger;

    public MaintenanceBackgroundService(IServiceScopeFactory scopeFactory, ILogger<MaintenanceBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

                var released = await mediator.Send(new ReleaseExpiredReservationsCommand(), stoppingToken);
                var completed = await mediator.Send(new AutoCompleteOrdersCommand(), stoppingToken);

                if (released > 0 || completed > 0)
                    _logger.LogInformation("Maintenance: released {Released} reservations, completed {Completed} orders.",
                        released, completed);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Maintenance background job failed.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}

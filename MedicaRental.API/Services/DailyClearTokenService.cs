using MedicaRental.DAL.UnitOfWork;

namespace MedicaRental.API.Services;

public class DailyClearTokenService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DailyClearTokenService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext from the service scope
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                if (unitOfWork is not null)
                {
                    var tokens = await unitOfWork.RefreshToken.FindAllAsync(predicate: t => t.RevokedOn != null, disableTracking: false);

                    var isDeleted = unitOfWork.RefreshToken.DeleteRange(tokens);

                    if (isDeleted)
                    {
                        unitOfWork.Save();
                    }
                }


            }

            // Sleep for 24 hours
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}

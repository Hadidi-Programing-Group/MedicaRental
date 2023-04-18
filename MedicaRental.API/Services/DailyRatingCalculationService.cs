using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace MedicaRental.API.Services;

public class DailyRatingCalculationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DailyRatingCalculationService(IServiceProvider serviceProvider)
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

                var items = await unitOfWork.Items.GetAllAsync(disableTracking: false);
                var clients = await unitOfWork.Clients.GetAllAsync(disableTracking: false);
                var reviews = await unitOfWork.Reviews.GetAllAsync();

                // Update the ratings for all items
                foreach (var item in items)
                {
                    var itemsReviews = reviews.Where(r => r.ItemId == item.Id);
                    item.Rating = itemsReviews.Any() ? (int)itemsReviews.Average(r => r.Rating) : 0;
                }

                foreach (var client in clients)
                {
                    var itemsReviews = reviews.Where(r => r.ClientId == client.Id);
                    client.Rating = itemsReviews.Any() ? (int)itemsReviews.Average(r => r.Rating) : 0;
                }

                unitOfWork.Save();

                // Save changes to the database
            }

            // Sleep for 24 hours
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }


}

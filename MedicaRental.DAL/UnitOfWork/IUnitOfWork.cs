using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IEntityRepo<Item> Items { get; }
    IEntityRepo<Category> Categories { get; }
    IEntityRepo<SubCategory> SubCategories {get;}
    IEntityRepo<Client> Clients {get;}
    IEntityRepo<AppUser> Admins {get;}
    IEntityRepo<Message> Messages {get;}
    IEntityRepo<Report> Reports {get;}
    IEntityRepo<Review> Reviews {get;}
    IEntityRepo<Brand> Brands {get; }
    IEntityRepo<RentOperation> RentOperations { get;}
    IEntityRepo<ReportAction> ReportActions { get; }

    IEntityRepo<RefreshToken> RefreshToken { get; }


    int Save();
}

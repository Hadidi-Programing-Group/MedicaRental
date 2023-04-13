using MedicaRental.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IItemsRepo Items { get; }
    ICategoriesRepo Categories { get; }
    ISubCategoriesRepo SubCategories {get;}
    IClientsRepo Clients {get;}
    IAdminsRepo Admins {get;}
    IMessagesRepo Messages {get;}
    IReportsRepo Reports {get;}
    IReviewsRepo Reviews {get;}
    IAccountsRepo Accounts {get;}

    int Save();
}

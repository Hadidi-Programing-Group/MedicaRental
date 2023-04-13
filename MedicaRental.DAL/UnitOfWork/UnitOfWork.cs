using MedicaRental.DAL.Context;
using MedicaRental.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    #region Private Fields
    private readonly MedicaRentalDbContext _context;
    private IItemsRepo? _items;
    private ICategoriesRepo? _categories;
    private ISubCategoriesRepo? _subCategories;
    private IClientsRepo? _clients;
    private IAdminsRepo? _admins;
    private IMessagesRepo? _messages;
    private IReportsRepo? _reports;
    private IReviewsRepo? _reviews;
    private IAccountsRepo? _accounts;
    #endregion

    public UnitOfWork(MedicaRentalDbContext context)
    {
        _context = context;
    }

    #region Repos Properties
    /**
     * Lazy Loading For Repositories
     * Will be initialized only when used
     */
    public IItemsRepo Items => _items ??= new ItemsRepo(_context);

    public ICategoriesRepo Categories => _categories ??= new CategoriesRepo(_context);

    public ISubCategoriesRepo SubCategories => _subCategories ??= new SubCategoriesRepo(_context);

    public IClientsRepo Clients => _clients ??= new ClientsRepo(_context);

    public IAdminsRepo Admins => _admins ??= new AdminsRepo(_context);

    public IMessagesRepo Messages => _messages ??= new MessagesRepo(_context);

    public IReportsRepo Reports => _reports ??= new ReportsRepo(_context);

    public IReviewsRepo Reviews => _reviews ??= new ReviewsRepo(_context);

    public IAccountsRepo Accounts => _accounts ??= new AccountsRepo(_context);
    #endregion

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public int Save()
    {
        return _context.SaveChanges();
    }
}

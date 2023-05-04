using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    #region Private Fields
    private readonly MedicaRentalDbContext _context;
    private IEntityRepo<Item>? _items;
    private IEntityRepo<Category>? _categories;
    private IEntityRepo<SubCategory>? _subCategories;
    private IEntityRepo<Client>? _clients;
    private IEntityRepo<AppUser>? _admins;
    private IEntityRepo<Message>? _messages;
    private IEntityRepo<Report>? _reports;
    private IEntityRepo<Review>? _reviews;
    private IEntityRepo<Brand>? _brands;
    private IEntityRepo<RentOperation>? _rentOperations;
    private IEntityRepo<ReportAction>? _reportActions;

    private IEntityRepo<RefreshToken>? _refreshToken;
    private IEntityRepo<Transaction>? _transactions;

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
    public IEntityRepo<Item> Items => _items ??= new ItemsRepo(_context);
    public IEntityRepo<Category> Categories => _categories ??= new CategoriesRepo(_context);
    public IEntityRepo<SubCategory> SubCategories => _subCategories ??= new SubCategoriesRepo(_context);
    public IEntityRepo<Client> Clients => _clients ??= new ClientsRepo(_context);
    public IEntityRepo<AppUser> Admins => _admins ??= new AdminsRepo(_context);
    public IEntityRepo<Message> Messages => _messages ??= new MessagesRepo(_context);
    public IEntityRepo<Report> Reports => _reports ??= new ReportsRepo(_context);
    public IEntityRepo<Review> Reviews => _reviews ??= new ReviewsRepo(_context);
    public IEntityRepo<Brand> Brands => _brands ??= new BrandsRepo(_context);
    public IEntityRepo<RentOperation> RentOperations => _rentOperations ??= new RentOperationsRepo(_context);
    public IEntityRepo<RefreshToken> RefreshToken => _refreshToken ??= new RefreshTokenRepo(_context);
    public IEntityRepo<ReportAction> ReportActions => _reportActions ??= new ReportActionRepo(_context);
    public IEntityRepo<Transaction> Trasactions => _transactions ??= new TransactionsRepo(_context);

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

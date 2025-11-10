using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinalProject.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LukitasDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(LukitasDbContext context)
    {
        _context = context;
        Users = new GenericRepository<User>(_context);
        Roles = new GenericRepository<Role>(_context);
        Accounts = new GenericRepository<Account>(_context);
        Campaigns = new GenericRepository<Campaign>(_context);
        Products = new GenericRepository<Product>(_context);
        ProductTypes = new GenericRepository<ProductType>(_context);
        Suppliers = new GenericRepository<Supplier>(_context);
        SupplierTypes = new GenericRepository<SupplierType>(_context);
        Sales = new GenericRepository<Sale>(_context);
        SaleDetails = new GenericRepository<SaleDetail>(_context);
        Coupons = new GenericRepository<Coupon>(_context);
        MissionTemplates = new GenericRepository<MissionTemplate>(_context);
        UserMissions = new GenericRepository<UserMission>(_context);
        Achievements = new GenericRepository<Achievement>(_context);
        Transfers = new GenericRepository<Transfer>(_context);
    }

    public IGenericRepository<User> Users { get; }
    public IGenericRepository<Role> Roles { get; }
    public IGenericRepository<Account> Accounts { get; }
    public IGenericRepository<Campaign> Campaigns { get; }
    public IGenericRepository<Product> Products { get; }
    public IGenericRepository<ProductType> ProductTypes { get; }
    public IGenericRepository<Supplier> Suppliers { get; }
    public IGenericRepository<SupplierType> SupplierTypes { get; }
    public IGenericRepository<Sale> Sales { get; }
    public IGenericRepository<SaleDetail> SaleDetails { get; }
    public IGenericRepository<Coupon> Coupons { get; }
    public IGenericRepository<MissionTemplate> MissionTemplates { get; }
    public IGenericRepository<UserMission> UserMissions { get; }
    public IGenericRepository<Achievement> Achievements { get; }
    public IGenericRepository<Transfer> Transfers { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

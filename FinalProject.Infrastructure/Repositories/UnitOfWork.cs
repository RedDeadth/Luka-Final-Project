using FinalProject.Domain.Entities;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LukitasDbContext _context;

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

    /// <summary>
    /// Ejecuta una operación dentro de una transacción con soporte para reintentos.
    /// Compatible con MySqlRetryingExecutionStrategy.
    /// </summary>
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await operation();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    /// <summary>
    /// Ejecuta una operación dentro de una transacción con soporte para reintentos.
    /// Compatible con MySqlRetryingExecutionStrategy.
    /// </summary>
    public async Task ExecuteInTransactionAsync(Func<Task> operation)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

using FinalProject.Domain.Entities;

namespace FinalProject.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<Account> Accounts { get; }
    IGenericRepository<Campaign> Campaigns { get; }
    IGenericRepository<Product> Products { get; }
    IGenericRepository<ProductType> ProductTypes { get; }
    IGenericRepository<Supplier> Suppliers { get; }
    IGenericRepository<SupplierType> SupplierTypes { get; }
    IGenericRepository<Sale> Sales { get; }
    IGenericRepository<SaleDetail> SaleDetails { get; }
    IGenericRepository<Coupon> Coupons { get; }
    IGenericRepository<MissionTemplate> MissionTemplates { get; }
    IGenericRepository<UserMission> UserMissions { get; }
    IGenericRepository<Achievement> Achievements { get; }
    IGenericRepository<Transfer> Transfers { get; }
    
    Task<int> SaveChangesAsync();
    
    /// <summary>
    /// Ejecuta una operación dentro de una transacción con soporte para reintentos.
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
    
    /// <summary>
    /// Ejecuta una operación dentro de una transacción con soporte para reintentos.
    /// </summary>
    Task ExecuteInTransactionAsync(Func<Task> operation);
}

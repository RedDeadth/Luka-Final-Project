using FinalProject.Infrastructure;

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
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

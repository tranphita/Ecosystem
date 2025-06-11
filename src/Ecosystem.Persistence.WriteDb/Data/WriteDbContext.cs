using Ecosystem.Domain.Aggregates.Role;
using Ecosystem.Domain.Aggregates.User;
using Ecosystem.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;


namespace Ecosystem.Persistence.WriteDb.Data;
/// <summary>
/// DbContext dùng cho ghi dữ liệu (WriteDbContext).
/// Chịu trách nhiệm quản lý các entity và phát hành các Domain Events trước khi lưu thay đổi vào cơ sở dữ liệu.
/// </summary>
public class WriteDbContext : DbContext
{
    private readonly IPublisher _publisher;

    /// <summary>
    /// Khởi tạo WriteDbContext với các tùy chọn và publisher để phát hành Domain Events.
    /// </summary>
    /// <param name="options">Tùy chọn cấu hình DbContext.</param>
    /// <param name="publisher">Đối tượng phát hành sự kiện miền (Domain Events).</param>
    public WriteDbContext(DbContextOptions<WriteDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    /// <summary>
    /// Cấu hình mô hình dữ liệu, tự động áp dụng tất cả các IEntityTypeConfiguration trong Assembly hiện tại.
    /// </summary>
    /// <param name="modelBuilder">Đối tượng ModelBuilder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tự động áp dụng tất cả các IEntityTypeConfiguration trong Assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Ghi đè SaveChangesAsync để phát hành các Domain Events trước khi lưu thay đổi vào database.
    /// </summary>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    /// <returns>Số lượng bản ghi bị ảnh hưởng.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Lấy và phát hành các Domain Events trước khi lưu vào DB
        await PublishDomainEvents(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Lấy tất cả các Aggregate Root có Domain Events và phát hành các sự kiện này.
    /// </summary>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    private async Task PublishDomainEvents(CancellationToken cancellationToken)
    {
        var aggregateRoots = ChangeTracker
            .Entries<AggregateRoot<object>>() // Lấy tất cả các Aggregate Root bị thay đổi
            .Where(entry => entry.Entity.GetDomainEvents().Count != 0)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(entry => entry.Entity.GetDomainEvents())
            .ToList();

        aggregateRoots.ForEach(entry => entry.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}

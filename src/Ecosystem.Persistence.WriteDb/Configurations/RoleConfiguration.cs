using Ecosystem.Domain.Aggregates.Role;
using Ecosystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Ecosystem.Persistence.WriteDb.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => new RoleId(value));

        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(r => r.Name).IsUnique();

        // Lưu trữ danh sách Permission dưới dạng một cột JSON
        // Đây là cách hiệu quả để lưu một danh sách các giá trị đơn giản
        builder.Property(r => r.Permissions)
            .HasConversion(
                p => JsonSerializer.Serialize(p, (JsonSerializerOptions?)null),
                p => JsonSerializer.Deserialize<HashSet<Permission>>(p, (JsonSerializerOptions?)null) ?? new HashSet<Permission>());
    }
}

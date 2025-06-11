using Ecosystem.Domain.Aggregates.Role;
using Ecosystem.Domain.Aggregates.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecosystem.Persistence.WriteDb.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        // UserId và RoleId conversion
        builder.Property(ur => ur.UserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(ur => ur.RoleId)
            .HasConversion(id => id.Value, value => new RoleId(value))
            .IsRequired();

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}

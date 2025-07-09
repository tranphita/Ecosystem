using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Ecosystem.SmartBox.Entities;

namespace Ecosystem.SmartBox.EntityFrameworkCore;

public static class SmartBoxDbContextModelCreatingExtensions
{
    public static void ConfigureSmartBox(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Cấu hình entity Company */
        builder.Entity<Company>(b =>
        {
            b.ToTable(SmartBoxDbProperties.DbTablePrefix + "Companies", SmartBoxDbProperties.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.TaxCode).HasMaxLength(50);
            b.Property(x => x.Address).HasMaxLength(500);
            b.Property(x => x.PhoneNumber).HasMaxLength(20);
            b.Property(x => x.Email).HasMaxLength(100);
            b.Property(x => x.Website).HasMaxLength(200);
            b.Property(x => x.Logo).HasMaxLength(1000);
            b.Property(x => x.Description).HasMaxLength(1000);

            // Indexes
            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.TaxCode);
            b.HasIndex(x => x.IsActive);
        });

        /* Cấu hình entity SmartBoxRole */
        builder.Entity<SmartBoxRole>(b =>
        {
            b.ToTable(SmartBoxDbProperties.DbTablePrefix + "Roles", SmartBoxDbProperties.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(500);

            // Indexes
            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.DisplayName);
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.DisplayOrder);
        });

        /* Cấu hình entity SmartBoxUser */
        builder.Entity<SmartBoxUser>(b =>
        {
            b.ToTable(SmartBoxDbProperties.DbTablePrefix + "Users", SmartBoxDbProperties.DbSchema);
            b.ConfigureByConvention();

            // Properties
            b.Property(x => x.AuthUserId).IsRequired();
            b.Property(x => x.UserName).IsRequired().HasMaxLength(100);
            b.Property(x => x.Email).IsRequired().HasMaxLength(200);
            b.Property(x => x.FullName).HasMaxLength(200);
            b.Property(x => x.PhoneNumber).HasMaxLength(20);
            b.Property(x => x.Avatar).HasMaxLength(1000);
            b.Property(x => x.Position).HasMaxLength(100);
            b.Property(x => x.Department).HasMaxLength(100);
            b.Property(x => x.EmployeeCode).HasMaxLength(50);
            b.Property(x => x.Salary).HasColumnType("decimal(18,2)");
            b.Property(x => x.Address).HasMaxLength(500);
            b.Property(x => x.Notes).HasMaxLength(1000);

            // Relations
            b.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            b.HasIndex(x => x.AuthUserId).IsUnique();
            b.HasIndex(x => x.UserName);
            b.HasIndex(x => x.Email);
            b.HasIndex(x => x.EmployeeCode);
            b.HasIndex(x => x.CompanyId);
            b.HasIndex(x => x.IsActive);
        });

        /* Cấu hình entity SmartBoxUserRole */
        builder.Entity<SmartBoxUserRole>(b =>
        {
            b.ToTable(SmartBoxDbProperties.DbTablePrefix + "UserRoles", SmartBoxDbProperties.DbSchema);
            b.ConfigureByConvention();

            // Composite Key
            b.HasKey(x => new { x.UserId, x.RoleId });

            // Relations
            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.RoleId);
            b.HasIndex(x => x.AssignedDate);
        });
    }
}

using Ecosystem.Domain.Aggregates.User;
using Ecosystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecosystem.Persistence.WriteDb.Configurations;
/// <summary>
/// Cấu hình ánh xạ thực thể User sang bảng "users" trong cơ sở dữ liệu.
/// Lưu ý: Các giá trị object như UserId, Email, FullName đều được ánh xạ thông qua Value Converter hoặc OwnsOne.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        #region users

        builder.ToTable("users");

        // --- Khóa chính ---
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => new UserId(value));

        // --- Identity & Core Info ---
        builder.Property(u => u.AuthId).IsRequired();
        builder.HasIndex(u => u.AuthId).IsUnique();

        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value)
            .HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.OwnsOne(u => u.FullName, fullNameBuilder =>
        {
            fullNameBuilder.Property(fn => fn.FirstName).HasColumnName("first_name").HasMaxLength(100);
            fullNameBuilder.Property(fn => fn.LastName).HasColumnName("last_name").HasMaxLength(100);
        });

        // --- Profile & Contact Info ---
        builder.Property(u => u.DisplayName).HasMaxLength(200);
        builder.Property(u => u.ProfilePictureUrl).HasMaxLength(2048);
        builder.Property(u => u.Bio).HasMaxLength(500);

        builder.Property(u => u.PhoneNumber)
            .HasConversion(
                phone => phone == null ? null : phone.Value,
                value => string.IsNullOrEmpty(value) ? null : PhoneNumber.Create(value).Value)
            .HasColumnName("phone_number").HasMaxLength(50);

        builder.Property(u => u.Gender).HasConversion<string>().HasMaxLength(20);
        builder.Property(u => u.DateOfBirth);

        // --- Status & Lifecycle ---
        builder.Property(u => u.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(u => u.CreatedOnUtc);
        builder.Property(u => u.LastModifiedOnUtc);
        builder.Property(u => u.LastLoginOnUtc);
        builder.Property(u => u.EmailVerifiedOnUtc);

        // --- Security ---
        builder.Property(u => u.LastPasswordChangedOnUtc);
        builder.Property(u => u.LockoutEnd);
        builder.Property(u => u.AccessFailedCount);

        // --- Relationships ---
        // Cấu hình mối quan hệ 1-N với UserRole
        builder.HasMany(u => u.UserRoles)
            .WithOne() // UserRole không có navigation property ngược lại User
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        #endregion
    }
}

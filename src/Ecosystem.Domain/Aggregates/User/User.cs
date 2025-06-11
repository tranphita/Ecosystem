using Ecosystem.Domain.Aggregates.Role;
using Ecosystem.Domain.Enums;
using Ecosystem.Domain.Events;
using Ecosystem.Domain.Primitives;
using Ecosystem.Domain.ValueObjects;
using Ecosystem.Shared;

namespace Ecosystem.Domain.Aggregates.User;

public sealed class User : AggregateRoot<UserId>
{
    #region --- Properties ---

    // --- Identity & Core Info ---
    public Guid AuthId { get; private set; }
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public FullName FullName { get; private set; }

    // --- Profile & Contact Info ---
    public string? DisplayName { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public string? Bio { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }

    // --- Status & Lifecycle ---
    public UserStatus Status { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? LastModifiedOnUtc { get; private set; }
    public DateTime? LastLoginOnUtc { get; private set; }
    public bool IsEmailVerified => EmailVerifiedOnUtc.HasValue;
    public DateTime? EmailVerifiedOnUtc { get; private set; }

    // --- Security ---
    public DateTime? LastPasswordChangedOnUtc { get; private set; }
    public DateTimeOffset? LockoutEnd { get; private set; }
    public int AccessFailedCount { get; private set; }

    // --- Relationships ---
    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    #endregion

    #pragma warning disable CS8618 // Báo cho compiler bỏ qua warning về non-nullable properties
    private User() : base(default) { } // CONSTRUCTOR RỖNG DÀNH RIÊNG CHO EF CORE
    #pragma warning restore CS8618

    // Private constructor để chỉ cho phép tạo User thông qua factory method "Register"
    private User(
        UserId id,
        Guid authId,
        string username,
        FullName fullName,
        Email email) : base(id)
    {
        AuthId = authId;
        Username = username;
        FullName = fullName;
        Email = email;
        CreatedOnUtc = DateTime.UtcNow;
        Status = UserStatus.Active; // Mặc định là Active
    }

    /// <summary>
    /// Factory method mới: Tạo một local profile cho user từ thông tin của IDP.
    /// Đây không phải là "đăng ký" theo kiểu truyền thống.
    /// Nó được gọi khi user đăng nhập vào service của chúng ta lần đầu tiên.
    /// </summary>
    public static Result<User> CreateFromIdp(Guid authId, string username, FullName fullName, Email email)
    {
        // Có thể thêm các quy tắc nghiệp vụ ở đây nếu cần.
        // Ví dụ: kiểm tra email có thuộc domain được phép không.

        if (string.IsNullOrWhiteSpace(username))
        {
            return Result<User>.Failure(new Error("User.EmptyUsername", "Username cannot be empty."));
        }

        var user = new User(UserId.New(), authId, username, fullName, email);

        // Phát sinh sự kiện: một local user profile đã được tạo (provisioned)
        user.RaiseDomainEvent(new UserRegisteredDomainEvent(Guid.NewGuid(), user.Id, user.AuthId, user.Email.Value));

        return user;
    }

    #region --- Behaviors (Hành vi nghiệp vụ) ---

    /// <summary>
    /// Đồng bộ thông tin profile từ IDP.
    /// Chỉ cập nhật nếu có sự thay đổi để tránh ghi thừa vào DB.
    /// </summary>
    public void SyncProfile(FullName newFullName, Email newEmail)
    {
        bool hasChanges = false;

        if (FullName != newFullName)
        {
            FullName = newFullName;
            hasChanges = true;
        }

        if (Email != newEmail)
        {
            Email = newEmail;
            hasChanges = true;
        }

        if (hasChanges)
        {
            LastModifiedOnUtc = DateTime.UtcNow;
        }
    }

    public void UpdateContactInfo(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber;
        LastModifiedOnUtc = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        if (!IsEmailVerified)
        {
            EmailVerifiedOnUtc = DateTime.UtcNow;
            LastModifiedOnUtc = DateTime.UtcNow;
            // Phát sinh sự kiện EmailVerifiedDomainEvent nếu cần
        }
    }

    public void TrackLogin()
    {
        LastLoginOnUtc = DateTime.UtcNow;
    }

    public void TrackFailedLoginAttempt()
    {
        if (LockoutEnd.HasValue && LockoutEnd > DateTimeOffset.UtcNow)
        {
            // User is locked out, do nothing
            return;
        }

        AccessFailedCount++;
        // Giả sử chính sách là khóa tài khoản sau 5 lần thất bại
        if (AccessFailedCount >= 5)
        {
            Status = UserStatus.LockedOut;
            LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15); // Khóa trong 15 phút
            // Phát sinh sự kiện UserLockedOutDomainEvent
        }
    }

    public void ResetLockout()
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
        Status = UserStatus.Active; // Mở khóa tài khoản
    }

    /// <summary>
    /// Ghi nhận rằng mật khẩu của người dùng đã được thay đổi thành công từ IDP.
    /// Phương thức này được gọi bởi Application Layer sau khi có xác nhận từ IDP.
    /// </summary>
    public void NotifyPasswordChanged()
    {
        LastPasswordChangedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new UserPasswordChangedDomainEvent(Guid.NewGuid(), Id));
    }

    public Result AddRole(RoleId roleId)
    {
        if (_userRoles.Any(ur => ur.RoleId == roleId))
        {
            return Result.Failure(new Error("User.RoleAlreadyExists", "User already has this role."));
        }

        var userRole = UserRole.Create(Id, roleId);
        _userRoles.Add(userRole);

        // Có thể phát sinh sự kiện UserRoleAssignedDomainEvent ở đây
        // RaiseDomainEvent(...);

        return Result.Success();
    }

    public Result RemoveRole(RoleId roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole is null)
        {
            return Result.Failure(new Error("User.RoleNotFound", "User does not have this role."));
        }

        _userRoles.Remove(userRole);

        // Có thể phát sinh sự kiện UserRoleRemovedDomainEvent ở đây
        // RaiseDomainEvent(...);

        return Result.Success();
    }

    #endregion
}

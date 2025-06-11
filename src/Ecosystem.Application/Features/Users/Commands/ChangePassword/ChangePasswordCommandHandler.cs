using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Application.Abstractions.Identity;
using Ecosystem.Shared;
using MediatR;

namespace Ecosystem.Application.Features.Users.Commands.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin user từ DB của service để có AuthId
        var user = await _unitOfWork.UserRepository.GetByIdAsync(new(request.UserId), cancellationToken);

        if (user is null)
        {
            return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        // 2. Gọi sang IdentityService để thực hiện đổi mật khẩu trên IDP
        var changePasswordResult = await _identityService.ChangePasswordAsync(
            user.AuthId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        // 3. Nếu đổi mật khẩu ở IDP thất bại, trả về lỗi ngay lập tức
        if (changePasswordResult.IsFailure)
        {
            return changePasswordResult;
        }

        // 4. (Tùy chọn) Nếu thành công, ghi nhận sự kiện vào domain của chúng ta
        user.NotifyPasswordChanged();

        // Vì command này không được bọc bởi TransactionBehavior,
        // Tự gọi SaveChanges ở đây để lưu lại thay đổi.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Trả về thành công
        return Result.Success();
    }
}
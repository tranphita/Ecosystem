namespace Ecosystem.Shared;

/// <summary>
/// Đại diện cho kết quả của một thao tác, có thể thành công hoặc thất bại.
/// </summary>
/// <remarks>
/// Lớp này dùng để đóng gói kết quả trả về của một thao tác, bao gồm trạng thái thành công/thất bại và thông tin lỗi (nếu có).
/// </remarks>
public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

/// <summary>
/// Đại diện cho kết quả của một thao tác với giá trị trả về.
/// </summary>
/// <typeparam name="TValue">Kiểu dữ liệu của giá trị trả về khi thao tác thành công.</typeparam>
/// <remarks>
/// Lớp này mở rộng từ <see cref="Result"/>, cho phép trả về giá trị khi thao tác thành công.
/// </remarks>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
    public new static Result<TValue> Failure(Error error) => new(default, false, error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}
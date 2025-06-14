namespace Ecosystem.Application.Common.Exceptions;

/// <summary>
/// Exception được ném khi một tài nguyên cụ thể không được tìm thấy.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
} 
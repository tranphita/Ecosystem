using Ecosystem.Domain.Aggregates.Role;

namespace Ecosystem.Application.Abstractions.Data;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    // Có thể thêm các phương thức khác sau này nếu cần
    // Task AddAsync(Role role, CancellationToken cancellationToken = default);
}

using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Domain.Aggregates.Role;
using Ecosystem.Persistence.WriteDb.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecosystem.Persistence.WriteDb.Repositories;

internal sealed class RoleRepository : IRoleRepository
{
    private readonly WriteDbContext _context;

    public RoleRepository(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
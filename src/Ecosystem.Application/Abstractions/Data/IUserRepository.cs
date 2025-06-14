﻿using Ecosystem.Domain.Aggregates.User;
using Ecosystem.Domain.ValueObjects;

namespace Ecosystem.Application.Abstractions.Data;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    
    Task<User?> GetByAuthIdAsync(Guid authId, CancellationToken cancellationToken = default);

    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);
}

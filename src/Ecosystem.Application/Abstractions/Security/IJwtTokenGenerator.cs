using Ecosystem.Domain.Aggregates.User;

namespace Ecosystem.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}

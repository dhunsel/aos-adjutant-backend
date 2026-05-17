#pragma warning disable MA0048
using AosAdjutant.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Users;

public sealed record ExternalIdentity(
    string Provider,
    string Subject,
    string Username,
    string Email
);

public sealed class UserService(ApplicationDbContext context, ILogger<UserService> logger)
{
    public async Task<User> ResolveExternalIdentity(ExternalIdentity identity)
    {
        var user = await context.Users.FirstOrDefaultAsync(u =>
            u.Subject == identity.Subject && u.IdentityProvider == identity.Provider
        );

        if (user is null)
        {
            var newUser = context.Users.Add(
                new User
                {
                    Subject = identity.Subject,
                    IdentityProvider = identity.Provider,
                    Username = identity.Username,
                    Email = identity.Email,
                    PublicId = Guid.NewGuid(),
                }
            );

            try
            {
                await context.SaveChangesAsync();

                logger.Log_UserCreated(newUser.Entity.UserId);

                return newUser.Entity;
            }
            catch (DbUpdateException ex) when (ex.IsUniqueViolation())
            {
                newUser.State = EntityState.Detached;

                user = await context.Users.FirstAsync(u =>
                    u.Subject == identity.Subject && u.IdentityProvider == identity.Provider
                );
            }
        }

        if (
            !user.Email.Equals(identity.Email, StringComparison.Ordinal)
            || !user.Username.Equals(identity.Username, StringComparison.Ordinal)
        )
        {
            user.Username = identity.Username;
            user.Email = identity.Email;

            await context.SaveChangesAsync();

            logger.Log_UserUpdated(user.UserId);
        }

        return user;
    }
}

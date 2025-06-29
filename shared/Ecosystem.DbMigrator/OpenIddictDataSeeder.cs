using JetBrains.Annotations;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using Ecosystem.DbMigrator.Model;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Ecosystem.DbMigrator;

public class OpenIddictDataSeeder(
    IAbpApplicationManager applicationManager,
    IOpenIddictScopeManager scopeManager,
    IPermissionDataSeeder permissionDataSeeder,
    IStringLocalizer<OpenIddictResponse> l,
    IConfiguration configuration,
    ICurrentTenant currentTenant
) : ITransientDependency
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IAbpApplicationManager _applicationManager = applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager = scopeManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder = permissionDataSeeder;
    private readonly IStringLocalizer<OpenIddictResponse> L = l;

    [UnitOfWork]
    public virtual async Task SeedAsync()
    {
        using (_currentTenant.Change(null))
        {
            await CreateApiResourcesAsync();
            await CreateClientsAsync();
        }
    }

    private async Task CreateClientsAsync()
    {
        var clients = _configuration.GetSection("Clients").Get<List<ServiceClient>>() ?? [];
        var commonScopes = new[]
        {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles,
            "offline_access",
        };

        foreach (var client in clients)
        {
            await CreateClientAsync(
                client.ClientId,
                displayName: client.ClientId,
                secret: client.ClientSecret,
                scopes: commonScopes.Union(client.Scopes).ToList(),
                grantTypes: [.. client.GrantTypes],
                redirectUris: client.RedirectUris,
                postLogoutRedirectUris: client.PostLogoutRedirectUris,
                consentType: OpenIddictConstants.ConsentTypes.Implicit
            );
        }
    }

    private async Task CreateApiResourcesAsync()
    {
        var apiResources = _configuration.GetSection("ApiResource").Get<string[]>() ?? [];

        foreach (var item in apiResources)
        {
            await CreateApiResourceAsync(item);
        }
    }

    private async Task CreateApiResourceAsync(string name)
    {
        if (await _scopeManager.FindByNameAsync(name) == null)
        {
            await _scopeManager.CreateAsync(
                new OpenIddictScopeDescriptor
                {
                    Name = name,
                    DisplayName = name + " API",
                    Resources = { name },
                }
            );
        }
    }

    private async Task CreateClientAsync(
        [NotNull] string name,
        string displayName,
        [NotNull] string consentType,
        string? secret,
        List<string> grantTypes,
        List<string> scopes,
        string[]? redirectUris = null,
        string[]? postLogoutRedirectUris = null,
        List<string>? permissions = null
    )
    {
        var type = string.IsNullOrWhiteSpace(secret)
            ? OpenIddictConstants.ClientTypes.Public
            : OpenIddictConstants.ClientTypes.Confidential;

        if (
            !string.IsNullOrEmpty(secret)
            && string.Equals(
                type,
                OpenIddictConstants.ClientTypes.Public,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BusinessException(L["NoClientSecretCanBeSetForPublicApplications"]);
        }

        if (
            string.IsNullOrEmpty(secret)
            && string.Equals(
                type,
                OpenIddictConstants.ClientTypes.Confidential,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BusinessException(L["TheClientSecretIsRequiredForConfidentialApplications"]);
        }

        if (
            !string.IsNullOrEmpty(name)
            && await _applicationManager.FindByClientIdAsync(name) != null
        )
        {
            return;
        }

        var client = await _applicationManager.FindByClientIdAsync(name);
        if (client == null)
        {
            var application = new OpenIddictApplicationDescriptor
            {
                ClientId = name,
                ApplicationType = type,
                ClientSecret = secret,
                ConsentType = consentType,
                DisplayName = displayName,
            };

            Check.NotNullOrEmpty(grantTypes, nameof(grantTypes));
            Check.NotNullOrEmpty(scopes, nameof(scopes));

            if (
                new[]
                {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.Implicit,
                }.All(grantTypes.Contains)
            )
            {
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken
                );
                if (
                    string.Equals(
                        type,
                        OpenIddictConstants.ClientTypes.Public,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken
                    );
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.ResponseTypes.CodeToken
                    );
                }
            }

            application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);

            foreach (var grantType in grantTypes)
            {
                if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode)
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode
                    );
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                }

                if (
                    grantType
                    is OpenIddictConstants.GrantTypes.AuthorizationCode
                        or OpenIddictConstants.GrantTypes.Implicit
                )
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.Endpoints.Authorization
                    );
                }

                if (
                    grantType
                    is OpenIddictConstants.GrantTypes.AuthorizationCode
                        or OpenIddictConstants.GrantTypes.ClientCredentials
                        or OpenIddictConstants.GrantTypes.Password
                        or OpenIddictConstants.GrantTypes.RefreshToken
                        or OpenIddictConstants.GrantTypes.DeviceCode
                )
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.Endpoints.Revocation
                    );
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.Endpoints.Introspection
                    );
                }

                if (grantType == OpenIddictConstants.GrantTypes.ClientCredentials)
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
                    );
                }

                if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.GrantTypes.Implicit
                    );
                }

                if (grantType == OpenIddictConstants.GrantTypes.Password)
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.GrantTypes.Password
                    );
                }

                if (grantType == OpenIddictConstants.GrantTypes.RefreshToken)
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken
                    );
                }

                if (grantType == OpenIddictConstants.GrantTypes.DeviceCode)
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.GrantTypes.DeviceCode
                    );
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
                }

                if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.ResponseTypes.IdToken
                    );
                    if (
                        string.Equals(
                            type,
                            OpenIddictConstants.ClientTypes.Public,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        application.Permissions.Add(
                            OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken
                        );
                        application.Permissions.Add(
                            OpenIddictConstants.Permissions.ResponseTypes.Token
                        );
                    }
                }
            }

            var buildInScopes = new[]
            {
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Phone,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                "offline_access",
            };

            foreach (var scope in scopes)
            {
                if (buildInScopes.Contains(scope))
                {
                    application.Permissions.Add(scope);
                }
                else
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.Prefixes.Scope + scope
                    );
                }
            }

            if (redirectUris != null)
            {
                foreach (var redirectUri in redirectUris)
                {
                    if (!redirectUri.IsNullOrEmpty())
                    {
                        if (
                            !Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri)
                            || !uri.IsWellFormedOriginalString()
                        )
                        {
                            throw new BusinessException(L["InvalidRedirectUri", redirectUri]);
                        }

                        if (application.RedirectUris.All(x => x != uri))
                        {
                            application.RedirectUris.Add(uri);
                        }
                    }
                }
            }

            if (postLogoutRedirectUris != null)
            {
                foreach (var postLogoutRedirectUri in postLogoutRedirectUris)
                {
                    if (!postLogoutRedirectUri.IsNullOrEmpty())
                    {
                        if (
                            !Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri)
                            || !uri.IsWellFormedOriginalString()
                        )
                        {
                            throw new BusinessException(
                                L["InvalidPostLogoutRedirectUri", postLogoutRedirectUri]
                            );
                        }

                        if (application.PostLogoutRedirectUris.All(x => x != uri))
                        {
                            application.PostLogoutRedirectUris.Add(uri);
                        }
                    }
                }
            }

            if (permissions != null)
            {
                await _permissionDataSeeder.SeedAsync(
                    ClientPermissionValueProvider.ProviderName,
                    name,
                    permissions,
                    null
                );
            }

            await _applicationManager.CreateAsync(application);
        }
    }
}

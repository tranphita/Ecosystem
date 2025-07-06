using System.Collections.Generic;
using System.Linq;

namespace Ecosystem.Administration.OpenIddict
{
    public static class OpenIddictPermissionConverter
    {
        public static List<string> ConvertPermissionsToGrantTypes(List<string> permissions)
        {
            var grantTypes = new List<string>();

            if (permissions.Contains("gt:authorization_code"))
            {
                grantTypes.Add(OpenIddictConsts.GrantTypes.AuthorizationCode);
            }
            if (permissions.Contains("gt:client_credentials"))
            {
                grantTypes.Add(OpenIddictConsts.GrantTypes.ClientCredentials);
            }
            if (permissions.Contains("gt:refresh_token"))
            {
                grantTypes.Add(OpenIddictConsts.GrantTypes.RefreshToken);
            }
            if (permissions.Contains("gt:password"))
            {
                grantTypes.Add(OpenIddictConsts.GrantTypes.Password);
            }
            if (permissions.Contains("gt:implicit"))
            {
                grantTypes.Add(OpenIddictConsts.GrantTypes.Implicit);
            }
            if (permissions.Contains("gt:urn:ietf:params:oauth:grant-type:device_code"))
            {
                grantTypes.Add(OpenIddictConsts.GrantTypes.DeviceCode);
            }

            return grantTypes;
        }

        public static List<string> ConvertPermissionsToResponseTypes(List<string> permissions)
        {
            var responseTypes = new List<string>();

            if (permissions.Contains("rst:code"))
            {
                responseTypes.Add(OpenIddictConsts.ResponseTypes.Code);
            }
            if (permissions.Contains("rst:token"))
            {
                responseTypes.Add(OpenIddictConsts.ResponseTypes.Token);
            }
            if (permissions.Contains("rst:id_token"))
            {
                responseTypes.Add(OpenIddictConsts.ResponseTypes.IdToken);
            }

            return responseTypes;
        }

        public static List<string> ConvertPermissionsToScopes(List<string> permissions)
        {
            var scopes = new List<string>();

            if (permissions.Contains("scp:openid"))
            {
                scopes.Add(OpenIddictConsts.Scopes.OpenId);
            }
            if (permissions.Contains("scp:profile"))
            {
                scopes.Add(OpenIddictConsts.Scopes.Profile);
            }
            if (permissions.Contains("scp:email"))
            {
                scopes.Add(OpenIddictConsts.Scopes.Email);
            }
            if (permissions.Contains("scp:address"))
            {
                scopes.Add(OpenIddictConsts.Scopes.Address);
            }
            if (permissions.Contains("scp:phone"))
            {
                scopes.Add(OpenIddictConsts.Scopes.Phone);
            }
            if (permissions.Contains("scp:roles"))
            {
                scopes.Add(OpenIddictConsts.Scopes.Roles);
            }

            return scopes;
        }
    }
} 
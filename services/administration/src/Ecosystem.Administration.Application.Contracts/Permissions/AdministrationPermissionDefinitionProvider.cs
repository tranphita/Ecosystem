using Ecosystem.Administration.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Ecosystem.Administration.Permissions;

public class AdministrationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var administrationGroup = context.AddGroup(
            AdministrationPermissions.GroupName,
            L("Permission:Administration")
        );
        var settingsPermissions = administrationGroup.AddPermission(
            AdministrationPermissions.Settings.Default,
            L("Permission:Administration:Settings")
        );
        settingsPermissions.AddChild(
            AdministrationPermissions.Settings.Update,
            L("Permission:Administration:Settings.Update")
        );

        // Định nghĩa permissions cho OpenIddict Management
        var openIddictPermissions = administrationGroup.AddPermission(
            AdministrationPermissions.OpenIddict.Default,
            L("Permission:Administration:OpenIddict")
        );

        // Permissions cho Applications
        var applicationsPermissions = openIddictPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Applications.Default,
            L("Permission:Administration:OpenIddict:Applications")
        );
        applicationsPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Applications.Create,
            L("Permission:Administration:OpenIddict:Applications.Create")
        );
        applicationsPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Applications.Update,
            L("Permission:Administration:OpenIddict:Applications.Update")
        );
        applicationsPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Applications.Delete,
            L("Permission:Administration:OpenIddict:Applications.Delete")
        );
        applicationsPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Applications.ManagePermissions,
            L("Permission:Administration:OpenIddict:Applications.ManagePermissions")
        );

        // Permissions cho Scopes
        var scopesPermissions = openIddictPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Scopes.Default,
            L("Permission:Administration:OpenIddict:Scopes")
        );
        scopesPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Scopes.Create,
            L("Permission:Administration:OpenIddict:Scopes.Create")
        );
        scopesPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Scopes.Update,
            L("Permission:Administration:OpenIddict:Scopes.Update")
        );
        scopesPermissions.AddChild(
            AdministrationPermissions.OpenIddict.Scopes.Delete,
            L("Permission:Administration:OpenIddict:Scopes.Delete")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AdministrationResource>(name);
    }
}

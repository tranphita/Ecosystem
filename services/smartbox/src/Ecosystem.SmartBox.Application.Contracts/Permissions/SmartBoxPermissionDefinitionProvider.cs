using Ecosystem.SmartBox.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Ecosystem.SmartBox.Permissions;

public class SmartBoxPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var smartBoxGroup = context.AddGroup(
            SmartBoxPermissions.GroupName,
            L("Permission:SmartBox")
        );
        var smartBoxPermissions = smartBoxGroup.AddPermission(
            SmartBoxPermissions.Issues.Default,
            L("Permission:SmartBox:Issues")
        );
        smartBoxPermissions.AddChild(
            SmartBoxPermissions.Issues.Create,
            L("Permission:SmartBox:Issues:Create")
        );
        smartBoxPermissions.AddChild(
            SmartBoxPermissions.Issues.Update,
            L("Permission:SmartBox:Issues:Update")
        );
        smartBoxPermissions.AddChild(
            SmartBoxPermissions.Issues.Delete,
            L("Permission:SmartBox:Issues:Delete")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SmartBoxResource>(name);
    }
}

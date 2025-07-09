using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Ecosystem.SmartBox;

/// <summary>
/// Permission Definition Provider cho SmartBox service
/// </summary>
public class SmartBoxPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var smartBoxGroup = context.AddGroup(
            SmartBoxPermissions.GroupName,
            L("Permission:SmartBox"));

        // Company permissions
        var companiesPermission = smartBoxGroup.AddPermission(
            SmartBoxPermissions.Companies.Default,
            L("Permission:CompanyManagement"));

        companiesPermission.AddChild(
            SmartBoxPermissions.Companies.Create,
            L("Permission:CompanyCreate"));

        companiesPermission.AddChild(
            SmartBoxPermissions.Companies.Edit,
            L("Permission:CompanyEdit"));

        companiesPermission.AddChild(
            SmartBoxPermissions.Companies.Delete,
            L("Permission:CompanyDelete"));

        // User permissions
        var usersPermission = smartBoxGroup.AddPermission(
            SmartBoxPermissions.Users.Default,
            L("Permission:UserManagement"));

        usersPermission.AddChild(
            SmartBoxPermissions.Users.Create,
            L("Permission:UserCreate"));

        usersPermission.AddChild(
            SmartBoxPermissions.Users.Edit,
            L("Permission:UserEdit"));

        usersPermission.AddChild(
            SmartBoxPermissions.Users.Delete,
            L("Permission:UserDelete"));

        usersPermission.AddChild(
            SmartBoxPermissions.Users.ManageRoles,
            L("Permission:UserManageRoles"));

        usersPermission.AddChild(
            SmartBoxPermissions.Users.ViewAllUsers,
            L("Permission:UserViewAll"));

        usersPermission.AddChild(
            SmartBoxPermissions.Users.ViewOwnCompanyUsers,
            L("Permission:UserViewOwnCompany"));

        // Role permissions
        var rolesPermission = smartBoxGroup.AddPermission(
            SmartBoxPermissions.Roles.Default,
            L("Permission:RoleManagement"));

        rolesPermission.AddChild(
            SmartBoxPermissions.Roles.Create,
            L("Permission:RoleCreate"));

        rolesPermission.AddChild(
            SmartBoxPermissions.Roles.Edit,
            L("Permission:RoleEdit"));

        rolesPermission.AddChild(
            SmartBoxPermissions.Roles.Delete,
            L("Permission:RoleDelete"));

        // Dashboard permissions
        var dashboardPermission = smartBoxGroup.AddPermission(
            SmartBoxPermissions.Dashboard.Default,
            L("Permission:Dashboard"));

        dashboardPermission.AddChild(
            SmartBoxPermissions.Dashboard.ViewStatistics,
            L("Permission:DashboardViewStatistics"));

        dashboardPermission.AddChild(
            SmartBoxPermissions.Dashboard.ViewReports,
            L("Permission:DashboardViewReports"));

        // Administration permissions
        var administrationPermission = smartBoxGroup.AddPermission(
            SmartBoxPermissions.Administration.Default,
            L("Permission:Administration"));

        administrationPermission.AddChild(
            SmartBoxPermissions.Administration.ManageSystem,
            L("Permission:AdministrationManageSystem"));

        administrationPermission.AddChild(
            SmartBoxPermissions.Administration.ViewLogs,
            L("Permission:AdministrationViewLogs"));

        administrationPermission.AddChild(
            SmartBoxPermissions.Administration.ManageSettings,
            L("Permission:AdministrationManageSettings"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SmartBoxResource>(name);
    }
} 
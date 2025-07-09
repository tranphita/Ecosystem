namespace Ecosystem.SmartBox;

/// <summary>
/// Định nghĩa các permission cho SmartBox service
/// </summary>
public static class SmartBoxPermissions
{
    public const string GroupName = "SmartBox";

    /// <summary>
    /// Permissions cho Company management
    /// </summary>
    public static class Companies
    {
        public const string Default = GroupName + ".Companies";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// Permissions cho User management
    /// </summary>
    public static class Users
    {
        public const string Default = GroupName + ".Users";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ManageRoles = Default + ".ManageRoles";
        public const string ViewAllUsers = Default + ".ViewAllUsers";
        public const string ViewOwnCompanyUsers = Default + ".ViewOwnCompanyUsers";
    }

    /// <summary>
    /// Permissions cho Role management
    /// </summary>
    public static class Roles
    {
        public const string Default = GroupName + ".Roles";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// Permissions cho Dashboard
    /// </summary>
    public static class Dashboard
    {
        public const string Default = GroupName + ".Dashboard";
        public const string ViewStatistics = Default + ".ViewStatistics";
        public const string ViewReports = Default + ".ViewReports";
    }

    /// <summary>
    /// Permissions cho Administration
    /// </summary>
    public static class Administration
    {
        public const string Default = GroupName + ".Administration";
        public const string ManageSystem = Default + ".ManageSystem";
        public const string ViewLogs = Default + ".ViewLogs";
        public const string ManageSettings = Default + ".ManageSettings";
    }

    /// <summary>
    /// Lấy tất cả permissions dưới dạng array
    /// </summary>
    public static string[] GetAll()
    {
        return new[]
        {
            // Companies
            Companies.Default,
            Companies.Create,
            Companies.Edit,
            Companies.Delete,
            
            // Users
            Users.Default,
            Users.Create,
            Users.Edit,
            Users.Delete,
            Users.ManageRoles,
            Users.ViewAllUsers,
            Users.ViewOwnCompanyUsers,
            
            // Roles
            Roles.Default,
            Roles.Create,
            Roles.Edit,
            Roles.Delete,
            
            // Dashboard
            Dashboard.Default,
            Dashboard.ViewStatistics,
            Dashboard.ViewReports,
            
            // Administration
            Administration.Default,
            Administration.ManageSystem,
            Administration.ViewLogs,
            Administration.ManageSettings
        };
    }
} 
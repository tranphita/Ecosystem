using Volo.Abp.Reflection;

namespace Ecosystem.Administration.Permissions;

public class AdministrationPermissions
{
    public const string GroupName = "Administration";

    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
        public const string Update = Default + ".Update";
    }

    // Permissions cho OpenIddict Management
    public static class OpenIddict
    {
        public const string Default = GroupName + ".OpenIddict";
        
        // Permissions cho Applications
        public static class Applications
        {
            public const string Default = OpenIddict.Default + ".Applications";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
            public const string ManagePermissions = Default + ".ManagePermissions";
        }
        
        // Permissions cho Scopes
        public static class Scopes
        {
            public const string Default = OpenIddict.Default + ".Scopes";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AdministrationPermissions));
    }
}

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

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AdministrationPermissions));
    }
}

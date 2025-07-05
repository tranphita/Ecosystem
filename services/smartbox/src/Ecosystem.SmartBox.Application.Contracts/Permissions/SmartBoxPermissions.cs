using Volo.Abp.Reflection;

namespace Ecosystem.SmartBox.Permissions;

public class SmartBoxPermissions
{
    public const string GroupName = "SmartBox";

    public static class Issues
    {
        public const string Default = GroupName + ".Issues";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(SmartBoxPermissions));
    }
}

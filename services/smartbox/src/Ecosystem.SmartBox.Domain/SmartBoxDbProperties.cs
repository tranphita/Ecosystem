namespace Ecosystem.SmartBox;

public static class SmartBoxDbProperties
{
    public const string ConnectionStringName = "SmartBox";
    public static string DbTablePrefix { get; set; } = "SmartBox";

    public static string DbSchema { get; set; } = null;
}

namespace Tyt.Infrastructure.Persistence;

public static class TytPaths
{
    public static string DataDirectory
    {
        get
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(root, "TYT");
        }
    }

    public static string DatabaseFile => Path.Combine(DataDirectory, "tyt.db");
}

namespace TimeCanvas.Core;

public static class AppPaths
{
    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TimeCanvas");

    public static string DatabaseFile
    {
        get
        {
            Directory.CreateDirectory(DataDirectory);
            return Path.Combine(DataDirectory, "data.db");
        }
    }
    
    public static string CrashLogFile
    {
        get
        {
            Directory.CreateDirectory(DataDirectory);
            return Path.Combine(DataDirectory, "crash.log");
        }
    }
}

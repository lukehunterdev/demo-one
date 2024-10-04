using System;
using System.Collections.ObjectModel;
using Dapper;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using LhDev.DemoOne.Services;

namespace LhDev.DemoOne.Data;

public static class DbManager
{
    public static ReadOnlyDictionary<string, string> DbValues { get; private set; }

    public static string DbPath { get; private set; } = null!;

    public static string ConnStr => $"Data Source={DbPath};";

    public static SQLiteConnection CreateDbConnection() => new(ConnStr);

    /// <summary>
    /// Call on application startup to check that the SQLite database already exists. If not create it here.
    /// </summary>
    public static void InitDatabase()
    {
        // Generate a path for the SQLite database file.
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                       ?? throw new Exception("Could not find executing path.");
        if (basePath.EndsWith(Path.DirectorySeparatorChar)) basePath = basePath[..^1];
        var baseDir = $"{basePath}{Path.DirectorySeparatorChar}data";
        basePath = $"{baseDir}{Path.DirectorySeparatorChar}sl.sqlite";

        DbPath = basePath;
        var createDb = !File.Exists(DbPath);
        if (createDb && !Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);

        if (createDb) SQLiteConnection.CreateFile(DbPath);

        using var dbConn = CreateDbConnection();
        dbConn.Open();

        if (createDb)
        {
            CreateDb(dbConn);
            CreateUser(dbConn, "User 1", "user1", "secret");
            CreateUser(dbConn, "User 2", "user2", "super-secret", true);
        }
    }

    private static void CreateUser(SQLiteConnection dbConn, string name, string username, string password, bool canEdit = false)
    {
        var userService = new DbUserService();
        var rootUser = userService.AddUser(username, name, password, canEdit, dbConn);
    }


    private static void CreateDb(SQLiteConnection dbConn)
    {
        dbConn.Execute(GetResourceString("CreateDb.sql"));
    }

    private static string GetResourceString(string name)
    {
        var ass = Assembly.GetExecutingAssembly();
        var resources = ass.GetManifestResourceNames().Where(r => r.EndsWith(name)).ToArray();

        if (resources.Length > 1)
            throw new Exception($"Resource name '{resources}' is ambiguous.");
        if (resources.Length < 1)
            throw new Exception($"Resource name '{resources}' could not be found.");
        var res = resources[0];

        using var stream = ass.GetManifestResourceStream(res);
        if (stream == null) throw new Exception($"Could not open stream for found resource '{res}'.");

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
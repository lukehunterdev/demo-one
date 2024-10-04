using System;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LhDev.DemoOne.ApiModels;
using LhDev.DemoOne.Data;
using LhDev.DemoOne.Exceptions;
using LhDev.DemoOne.ExtensionMethods;
using LhDev.DemoOne.Models;

namespace LhDev.DemoOne.Services;

public interface IDbUserService
{
    Task<User> AuthenticateUserAsync(string username, string password);

}

public class DbUserService : IDbUserService
{
    public Task<User> AuthenticateUserAsync(string username, string password)
    {
        var task = new Task<User>(() =>
        {
            using var dbConn = DbManager.CreateDbConnection();
            dbConn.Open();

            // Get User object or error out.
            var user = GetUser(dbConn, "Username = @username", new { username = username.ToLower() })
                       ?? throw DemoOneWebException.UserOrPasswordNotFound();

            // Get Password object or error out.
            var pass = GetPassword(dbConn, "UserId = @userId", new { userId = user.Id })
                       ?? throw DemoOneWebException.UserOrPasswordNotFound();

            // Calculate password hash, and check if the hashes match.
            if (CalculateHash(password, pass.Salt) != pass.Hash) throw DemoOneWebException.UserOrPasswordNotFound();

            // Validated, return user.
            return user;
        });

        task.Start();

        return task;
    }

    public User AddUser(string username, string name, string password, bool canEdit, SQLiteConnection dbConn)
    {
        if (UserExists(dbConn, "username = @username", new { username = username.ToLower() }))
            throw DemoOneWebException.UserExists(username);

        var def = new CommandDefinition("INSERT INTO User (Username, Name, CanEdit) VALUES (@username, @name, @canEdit)",
            new { username = username.ToLower(), name, canEdit });
        if (dbConn.Execute(def) != 1) throw DemoOneWebException.CouldNotCreateUser();

        // Get User object.
        var userId = dbConn.LastInsertRowId;
        var user = GetUser(dbConn, "Id = @id", new { id = userId })
                   ?? throw DemoOneWebException.UserOrPasswordNotFound();

        // Create a new Password record.
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
        var hash = CalculateHash(password, salt);
        def = new CommandDefinition("INSERT INTO Password (UserId, Salt, Hash) VALUES (@userId, @salt, @hash)",
            new { userId, salt, hash });
        if (dbConn.Execute(def) != 1) throw DemoOneWebException.CouldNotCreatePassword();

        return user;
    }

    internal static bool UserExists(SQLiteConnection dbConn, string whereClause, object? parameters = null)
    {
        var def = new CommandDefinition($"SELECT COUNT(Id) FROM User WHERE {whereClause}", parameters);

        return dbConn.ExecuteScalar<long>(def) == 1;
    }

    internal static User? GetUser(SQLiteConnection dbConn, string whereClause, object? parameters = null)
    {
        var def = new CommandDefinition($"SELECT * FROM User WHERE {whereClause}", parameters);

        return dbConn.Query<User>(def).FirstOrDefault() ;
    }

    private static Password? GetPassword(SQLiteConnection dbConn, string whereClause, object? parameters = null)
    {
        var def = new CommandDefinition($"SELECT * FROM Password WHERE {whereClause}", parameters);

        return dbConn.Query<Password>(def).FirstOrDefault();
    }

    private static string CalculateHash(string password, string salt)
    {
        using var sha = SHA512.Create() ?? throw new Exception();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{password}#{salt}"));
        var sb = new StringBuilder();

        foreach (var b in bytes) sb.Append($"{b:X2}");
        return sb.ToString();
    }
}
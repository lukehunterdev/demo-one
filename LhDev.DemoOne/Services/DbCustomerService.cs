using Dapper;
using LhDev.DemoOne.Data;
using LhDev.DemoOne.Exceptions;
using LhDev.DemoOne.Models;
using System;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Linq;

namespace LhDev.DemoOne.Services;


public interface IDbCustomerService
{
    Task<Customer> AddAsync(NewCustomer model);
    Task<int> GetCountAsync();
    Task<Customer[]> GetAsync(int offset, int count);
    Task<Customer> GetAsync(int id);
    Task<Customer> EditAsync(int id, NewCustomer model);
    Task DeleteAsync(int id);
}

public class DbCustomerService : IDbCustomerService
{

    public Customer Add(string firstName, string surname)
    {
        using var dbConn = DbManager.CreateDbConnection();
        dbConn.Open();

        return Add(firstName, surname, dbConn);
    }

    public Customer Add(string firstName, string surname, SQLiteConnection dbConn)
    {
        var def = new CommandDefinition("INSERT INTO Customer (FirstName, Surname) VALUES (@firstName, @surname)",
            new { firstName, surname });
        if (dbConn.Execute(def) != 1) throw DemoOneWebException.CouldNotCreateCustomer();

        // Get User object.
        var userId = dbConn.LastInsertRowId;
        var user = Get(dbConn, "Id = @id", new { id = userId })
                   ?? throw DemoOneWebException.CustomerNotFound();

        return user;
    }

    public Task<Customer> AddAsync(NewCustomer model)
        => AddAsync(model.FirstName, model.Surname);

    public Task<Customer> AddAsync(string firstName, string surname)
    {
        var task = new Task<Customer>(() => Add(firstName, surname));
        task.Start();

        return task;
    }



    public Task<int> GetCountAsync()
    {
        var task = new Task<int>(() =>
        {
            using var dbConn = DbManager.CreateDbConnection();
            dbConn.Open();

            var count = dbConn.ExecuteScalar("SELECT COUNT(Id) FROM Customer");

            return Convert.ToInt32(count);
        });
        task.Start();

        return task;
    }

    public Task<Customer[]> GetAsync(int offset, int count)
    {
        var task = new Task<Customer[]>(() =>
        {
            using var dbConn = DbManager.CreateDbConnection();
            dbConn.Open();

            return dbConn.Query<Customer>("SELECT * FROM Customer LIMIT @count OFFSET @offset", new { offset, count }).ToArray();
        });
        task.Start();

        return task;
    }

    public Task<Customer> GetAsync(int id)
    {
        var task = new Task<Customer>(() =>
        {
            using var dbConn = DbManager.CreateDbConnection();
            dbConn.Open();

            return Get(dbConn, "Id = @id", new { id })
                   ?? throw DemoOneWebException.ApiCustomerIdNotFound(id);
        });
        task.Start();

        return task;
    }

    internal static Customer? Get(SQLiteConnection dbConn, string whereClause, object? parameters = null)
    {
        var def = new CommandDefinition($"SELECT * FROM Customer WHERE {whereClause}", parameters);

        return dbConn.Query<Customer>(def).FirstOrDefault();
    }



    public Customer Edit(int id, string firstName, string surname)
    {
        using var dbConn = DbManager.CreateDbConnection();
        dbConn.Open();

        return Edit(id, firstName, surname, dbConn);
    }

    public Customer Edit(int id, string firstName, string surname, SQLiteConnection dbConn)
    {
        var def = new CommandDefinition("UPDATE Customer SET FirstName = @firstName, Surname = @surname WHERE Id = @id",
            new { id, firstName, surname });
        if (dbConn.Execute(def) != 1) throw DemoOneWebException.CouldNotCreateCustomer();

        // Get Customer object.
        var customer = Get(dbConn, "Id = @id", new { id })
                   ?? throw DemoOneWebException.CustomerNotFound();

        return customer;
    }

    public Task<Customer> EditAsync(int id, NewCustomer model)
        => EditAsync(id, model.FirstName, model.Surname);

    public Task<Customer> EditAsync(int id, string firstName, string surname)
    {
        var task = new Task<Customer>(() => Edit(id, firstName, surname));
        task.Start();

        return task;
    }

    public Task DeleteAsync(int id)
    {
        var task = new Task(() =>
        {
            using var dbConn = DbManager.CreateDbConnection();
            dbConn.Open();
            
            var def = new CommandDefinition("DELETE FROM Customer WHERE Id = @id", new { id });
            if (dbConn.Execute(def) != 1) throw DemoOneWebException.CouldNotDeleteCustomer(id);
        });

        task.Start();

        return task;
    }
}
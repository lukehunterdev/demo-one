using System;
using Microsoft.AspNetCore.Http;

namespace LhDev.DemoOne.Exceptions;

/// <summary>
/// A custom exception type created to facilitate clean and simple error handling.
/// </summary>
/// <remarks>
/// Designed to making exception throwing and handling easier. There are static methods that generate the required exception based
/// on given parameters, if any. This is done to allow easy reuse of various exceptions, and to allow the exception handler pipeline
/// to easily identify our own exception type, which are designed to provide the all the right information about the error.
/// </remarks>
public class DemoOneWebException : DemoOneException
{
    public int StatusCode { get; init; }


    #region Constructors

    protected DemoOneWebException(string? message) : this(message, null)
    {
    }

    protected DemoOneWebException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    #endregion


    #region Test exceptions

    public static DemoOneWebException TestException()
        => new("This is a test DemoOneWebException type.") { StatusCode = StatusCodes.Status401Unauthorized };

    #endregion


    #region Authorisation and authentication
    
    public static DemoOneWebException ApiUserIdNotFound(int id)
        => new($"User with id number '{id}' not found.") { StatusCode = StatusCodes.Status404NotFound };

    public static DemoOneWebException ApiUsernameNotFound(string username)
        => new($"Username '{username}' not found.") { StatusCode = StatusCodes.Status404NotFound };

    public static DemoOneWebException UserOrPasswordNotFound()
        => new("Username or password is incorrect.") { StatusCode = StatusCodes.Status401Unauthorized };

    public static DemoOneWebException UserExists(string username)
        => new($"The username '{username}' already exists.") { StatusCode = StatusCodes.Status400BadRequest };

    public static DemoOneWebException CouldNotCreateUser()
        => new("Could not create user.") { StatusCode = StatusCodes.Status500InternalServerError };

    public static DemoOneWebException CouldNotCreatePassword()
        => new("Could not create password.") { StatusCode = StatusCodes.Status500InternalServerError };

    #endregion

    
    #region Customer issues

    public static DemoOneWebException CouldNotCreateCustomer()
        => new("Could not create new customer.") { StatusCode = StatusCodes.Status500InternalServerError };
    public static DemoOneWebException CustomerNotFound()
        => new("Could not find newly created customer.") { StatusCode = StatusCodes.Status500InternalServerError };
    public static DemoOneWebException ApiCustomerIdNotFound(int id)
        => new($"Customer with id number '{id}' not found.") { StatusCode = StatusCodes.Status404NotFound };
    public static DemoOneWebException CouldNotDeleteCustomer(int id)
        => new($"Could not delete customer with id number '{id}'.") { StatusCode = StatusCodes.Status404NotFound };



    public static DemoOneWebException WebCouldNotGetCustomers()
        => new($"Could not get list of customers.") { StatusCode = StatusCodes.Status500InternalServerError };
    public static DemoOneWebException WebCouldNotFindCustomer(int id)
        => new($"Could not find customer with ID '{id}'.") { StatusCode = StatusCodes.Status404NotFound };

    #endregion
    

    #region User issues

    public static DemoOneWebException CouldNotUpdateUser(int id, string? extraInfo = null) 
        => new($"Could not update user with ID {id}. {extraInfo}".Trim());

    public static DemoOneWebException CouldNotFindUserId(int id) 
        => new($"Could not find user with ID {id}.");

    #endregion
    

    #region Not Authorised

    public static DemoOneWebException NotAuthorised(string? message = "You are not authorised to access this resource.")
        => new(message) { StatusCode = StatusCodes.Status403Forbidden };

    #endregion

}
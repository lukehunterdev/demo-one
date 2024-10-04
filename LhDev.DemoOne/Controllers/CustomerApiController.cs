using System;
using System.Threading.Tasks;
using LhDev.DemoOne.ApiModels;
using LhDev.DemoOne.Attributes;
using LhDev.DemoOne.ExtensionMethods;
using LhDev.DemoOne.Models;
using LhDev.DemoOne.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LhDev.DemoOne.Controllers;

[ApiException]
[ApiController]
[Route("api/customer")]
public class CustomerApiController(IDbCustomerService dbCustomerService) : ControllerBase
{
    /// <summary>
    /// Gets a collection of customers.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="size"></param>
    /// <returns>Upon success, a <see cref="PagedResponse{T}"/> object with the customer objects</returns>
    [ApiJwtAuthorise]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<Customer>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(GeneralResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GeneralResponse))]
    [HttpGet]
    public async Task<IActionResult> GetMany(int number = 0, int size = 100) =>
        Ok(new PagedResponse<Customer>
        {
            Page = number,
            PageSize = size,
            Total = (int)Math.Ceiling(await dbCustomerService.GetCountAsync() / (double)size),
            Records = await dbCustomerService.GetAsync(number * size, size),
        });


    /// <summary>
    /// Gets customer information by id number.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Upon success, a <see cref="Customer"/> object.</returns>
    [ApiJwtAuthorise]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Customer))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(GeneralResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GeneralResponse))]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) => Ok(await dbCustomerService.GetAsync(id));


    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="customer"><see cref="NewCustomer"/> object containing new customer details.</param>
    /// <returns>Upon success, a <see cref="GeneralResponse"/> with a message.</returns>
    [ApiJwtAuthorise(RequiresEdit = true)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(GeneralResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GeneralResponse))]
    [HttpPost]
    public async Task<IActionResult> Create(NewCustomer customer)
    {
        var customerObj = await dbCustomerService.AddAsync(customer);

        return this.OkGeneralResponse($"New customer '{customerObj}' added.");
    }


    /// <summary>
    /// Creates a new customer, or modifies an existing one.
    /// </summary>
    /// <param name="id">ID number of customer to edit. If omitted, creates a new customer.</param>
    /// <param name="customer"><see cref="NewCustomer"/> object containing customer details.</param>
    /// <returns>Upon success, a <see cref="GeneralResponse"/> with a message.</returns>
    [ApiJwtAuthorise(RequiresEdit = true)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(GeneralResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GeneralResponse))]
    [HttpPut("{id:int?}")]
    public async Task<IActionResult> CreateOrUpdate(int? id, NewCustomer customer)
    {
        if (id.HasValue)
        {
            await dbCustomerService.EditAsync(id.Value, customer);
            return this.OkGeneralResponse($"Customer ID '{id}' modified.");
        }

        var customerObj = await dbCustomerService.AddAsync(customer);
        return this.OkGeneralResponse($"New customer '{customerObj}' added.");
    }


    /// <summary>
    /// Deletes a customer
    /// </summary>
    /// <param name="id">ID number of customer to edit.</param>
    /// <returns>Upon success, a <see cref="GeneralResponse"/> with a message.</returns>
    [ApiJwtAuthorise(RequiresEdit = true)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(GeneralResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GeneralResponse))]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await dbCustomerService.DeleteAsync(id);
        
        return this.OkGeneralResponse($"Customer ID '{id}' deleted.");
    }
}
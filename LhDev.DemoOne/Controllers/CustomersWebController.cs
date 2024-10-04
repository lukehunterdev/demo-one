using System;
using System.Threading.Tasks;
using LhDev.DemoOne.Attributes;
using LhDev.DemoOne.Data;
using LhDev.DemoOne.Exceptions;
using LhDev.DemoOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LhDev.DemoOne.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("Customers")]
public class CustomersWebController(AppDbContext dbContext) : Controller
{
    [WebJwtAuthorise(RedirectUri = "/Home/Login")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var customers = await dbContext.Customers.ToListAsync();

        if (customers == null) throw DemoOneWebException.WebCouldNotGetCustomers();

        var currentUser = (User)HttpContext.Items["User"]!;
        
        return View((customers, currentUser.CanEdit));
    }

    [WebJwtAuthorise(RequiresEdit = true, RedirectUri = "/Home/Login")]
    [HttpGet("Add")]
    public IActionResult Add()
    {
        return View();
    }

    [WebJwtAuthorise(RequiresEdit = true, RedirectUri = "/Home/Login")]
    [HttpPost("Add")]
    public async Task<IActionResult> Add(Customer model)
    {
        await dbContext.Customers.AddAsync(model);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("List", "CustomersWeb");
    }


    [WebJwtAuthorise(RequiresEdit = true, RedirectUri = "/Home/Login")]
    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await dbContext.Customers.FindAsync(id);

        if (customer == null) throw DemoOneWebException.WebCouldNotFindCustomer(id);

        return View(customer);
    }


    [WebJwtAuthorise(RequiresEdit = true, RedirectUri = "/Home/Login")]
    [HttpPost("Edit/{id:int}")]
    public async Task<IActionResult> Edit(Customer model)
    {
        var customer = await dbContext.Customers.FindAsync(model.Id);

        if (customer != null)
        {
            customer.FirstName = model.FirstName;
            customer.Surname = model.Surname;

            await dbContext.SaveChangesAsync();
        }

        return RedirectToAction("List", "CustomersWeb");
    }

    [WebJwtAuthorise(RequiresEdit = true, RedirectUri = "/Home/Login")]
    [HttpGet("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await dbContext.Customers.FindAsync(id);

        if (customer == null) throw DemoOneWebException.WebCouldNotFindCustomer(id);

        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("List", "CustomersWeb");
    }
}
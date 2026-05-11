using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailCore.Services.Interfaces;
using RetailCore.Shared.Constants;

namespace RetailCore.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = RoleConstants.Admin)]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var result = await _userService.GetCustomersAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPatch("{id:guid}/toggle-active")]
    public async Task<IActionResult> ToggleActiveStatus(Guid id)
    {
        await _userService.ToggleActiveStatusAsync(id);
        return NoContent();
    }
}

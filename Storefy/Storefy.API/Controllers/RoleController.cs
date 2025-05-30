using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Services;

namespace Storefy.API.Controllers;

/// <summary>
/// Represents the Role API Controller class.
/// </summary>
[Route("roles/")]
[ApiController]
[Authorize(Policy = "Admin")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleController"/> class.
    /// </summary>
    /// <param name="roleService">Service to perform operations on Role.</param>
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// HTTP Get method to retrie all Roles.
    /// </summary>
    /// <returns>All the roles in the system.</returns>
    [HttpGet]
    public async Task<ActionResult<Role>> GetRoles()
    {
        var roles = await _roleService.GetAllRoles();

        return Ok(roles);
    }

    /// <summary>
    /// HTTP Get method to retrie all Roles associated to a user.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns>Roles associated with the user if found any.</returns>
    [HttpGet("users/{id}")]
    public async Task<ActionResult<Role>> GetRolesByUser(string id)
    {
        var rolesByUser = await _roleService
            .GetRolesByUser(id);

        return Ok(rolesByUser);
    }

    /// <summary>
    /// HTTP Post method to create a new Role.
    /// </summary>
    /// <param name="newRoleDto">Data Transfer Object of
    /// the Role that is to be created.</param>
    /// <returns>Newly created role.</returns>
    [HttpPost("add")]
    public async Task<ActionResult<Role>> CreateNewRole(CreateUpdateRoleDto newRoleDto)
    {
        var newRole = await _roleService.CreateNewRole(newRoleDto);

        return CreatedAtAction(nameof(CreateNewRole), newRole);
    }

    /// <summary>
    /// HTTP Delete mehtod to delete a Role by its Id.
    /// </summary>
    /// <param name="id">Role Identifier to be deleted.</param>
    [HttpDelete("remove/{id}")]
    public async Task<ActionResult<Role>> DeleteRole(string id)
    {
        var deletedRole = await _roleService.DeleteRole(id);

        return Ok(deletedRole);
    }

    /// <summary>
    /// HTTP Get method to retrieve all permissions.
    /// </summary>
    /// <returns>All the permissions in the system.</returns>
    [HttpGet("permissions")]
    public async Task<ActionResult<string>> GetPermissions()
    {
        var permissions = await _roleService.GetPermissions();

        return Ok(permissions);
    }

    /// <summary>
    /// HTTP Get method to retrieve role information by it's Id.
    /// </summary>
    /// <param name="id">Role identifier.</param>
    /// <returns>Roles associated with Id.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetRoleById(string id)
    {
        var selectedRole = await _roleService.GetRoleById(id);

        return Ok(selectedRole);
    }

    /// <summary>
    /// HTTP Get method to get permissions by Role.
    /// </summary>
    /// <param name="id">Role identifier.</param>
    /// <returns>All permissions associated with roles.</returns>
    [HttpGet("{id}/permissions")]
    public async Task<ActionResult<string>> GetRolePermissions(string id)
    {
        var rolePermissions = await _roleService.GetRolePermissions(id);

        return Ok(rolePermissions);
    }

    /// <summary>
    /// HTTP Put method to update Role.
    /// </summary>
    /// <param name="roleDto">Data Transfer Object of
    /// the Role that is to be updated.</param>
    /// <returns>Updated role.</returns>
    [HttpPut("update")]
    public async Task<ActionResult<Role>> UpdateRole(CreateUpdateRoleDto roleDto)
    {
        var updatedRole = await _roleService.UpdateRole(roleDto);

        return Ok(updatedRole);
    }
}

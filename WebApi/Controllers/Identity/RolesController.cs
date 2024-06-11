﻿using Application.Features.Identity.Roles.Commands;
using Application.Features.Identity.Roles.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    public class RolesController : AppBaseController<RolesController>
    {
        [HttpPost]
        [MustHavePermission(AppFeature.Roles, AppAction.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var response = await MediatorSender.Send(new CreateRoleCommand { RoleRequest = request });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [MustHavePermission(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoles()
        {
            var response = await MediatorSender.Send(new GetRolesQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut]
        [MustHavePermission(AppFeature.Roles, AppAction.Update)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
        {
            var response = await MediatorSender.Send(new UpdateRoleCommand { UpdateRole = request });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
    }
}
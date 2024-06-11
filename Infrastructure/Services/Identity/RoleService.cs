using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Infrastructure.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public RoleService(RoleManager<ApplicationRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request)
        {
            var roleExist = await _roleManager.FindByNameAsync(request.RoleName);
            if (roleExist is not null)
            {
                return await ResponseWrapper<string>.FailAsync("Role already exist");
            }

            var IdentityResult = await _roleManager.CreateAsync(new ApplicationRole
            {
                Name = request.RoleName,
                Description = request.Description
            });

            if (IdentityResult.Succeeded)
            {
                return await ResponseWrapper<string>.SuccessAsync("Role created successfully");
            }
            return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDesc(IdentityResult));
        }

        public async Task<IResponseWrapper> GetRolesAsync()
        {
            var allRoles = await _roleManager.Roles.ToListAsync();
            if (allRoles.Count > 0)
            {
                var mappedRoles = _mapper.Map<List<RoleResponse>>(allRoles);
                return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(mappedRoles);
            }
            return await ResponseWrapper<string>.FailAsync("No roles where found");
        }

        public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request)
        {
            var roleInDb = await _roleManager.FindByNameAsync(request.RoleId);
            if(roleInDb is not null)
            {
                if(roleInDb.Name == AppRoles.Admin)
                {
                    return await ResponseWrapper<string>.FailAsync("Admin role cannot be updated");
                }
                roleInDb.Name = request.RoleName;
                roleInDb.Description = request.RoleDescription;

                var isIdentityResult = await _roleManager.UpdateAsync(roleInDb);
                if(isIdentityResult.Succeeded)
                {
                    return await ResponseWrapper<string>.SuccessAsync("Role updated successfully");
                }
                return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDesc(isIdentityResult));
            }
            return await ResponseWrapper<string>.FailAsync("Role does not exist or cannot be updated");
        }

        private List<string> GetIdentityResultErrorDesc(IdentityResult identityResult)
        {
            var errorDescriptions = new List<string>();
            foreach (var error in identityResult.Errors)
            {
                errorDescriptions.Add(error.Description);
            }

            return errorDescriptions;
        }
    }
}

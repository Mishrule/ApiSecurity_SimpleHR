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
using Infrastructure.Context;
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
        private readonly ApplicationDbContext _context;

        public RoleService(RoleManager<ApplicationRole> roleManager, IMapper mapper, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
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
            if (roleInDb is not null)
            {
                if (roleInDb.Name == AppRoles.Admin)
                {
                    return await ResponseWrapper<string>.FailAsync("Admin role cannot be updated");
                }
                roleInDb.Name = request.RoleName;
                roleInDb.Description = request.RoleDescription;

                var isIdentityResult = await _roleManager.UpdateAsync(roleInDb);
                if (isIdentityResult.Succeeded)
                {
                    return await ResponseWrapper<string>.SuccessAsync("Role updated successfully");
                }
                return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDesc(isIdentityResult));
            }
            return await ResponseWrapper<string>.FailAsync("Role does not exist or cannot be updated");
        }

        public async Task<IResponseWrapper> GetRoleByIdAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);
            if (roleInDb is not null)
            {
                var mappedRole = _mapper.Map<RoleResponse>(roleInDb);
                return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedRole);
            }
            return await ResponseWrapper<string>.FailAsync("Role does not exist");
        }

        public async Task<IResponseWrapper> DeleteRoleAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);
            if (roleInDb is not null)
            {
                if (roleInDb.Name != AppRoles.Admin)
                {
                    var allUsers = await _userManager.Users.ToListAsync();
                    foreach (var user in allUsers)
                    {
                        if (await _userManager.IsInRoleAsync(user, roleInDb.Name))
                        {
                            return await ResponseWrapper.FailAsync($"Role: {roleInDb.Name} is currently assigned to a user.");
                        }
                    }
                    var identityResult = await _roleManager.DeleteAsync(roleInDb);
                    if (identityResult.Succeeded)
                    {
                        return await ResponseWrapper<string>.SuccessAsync("Role deleted successfully");
                    }
                    return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDesc(identityResult));
                }
                return await ResponseWrapper<string>.FailAsync("Admin role cannot be deleted");
            }
            return await ResponseWrapper<string>.FailAsync("Role does not exist");
        }

        public async Task<IResponseWrapper> GetPermissionAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);
            if (roleInDb is not null)
            {
                var allPermissions = AppPermissions.AllPermissions;
                var roleClaimResponse = new RoleClaimResponse
                {
                    Role = new()
                    {
                        Id = roleId,
                        Name = roleInDb.Name,
                        Description = roleInDb.Description
                    },
                    RoleClaims = new()
                };

                var currentRoleClaims = await GetAllClaimsForRoleAsync(roleId);
                var allPermissionsNames = allPermissions.Select(p => p.Name).ToList();
                var currentRoleClaimsValues = currentRoleClaims.Select(c => c.ClaimValue).ToList();

                var currentlyAssignedRoleClaimsNames = allPermissionsNames.Intersect(currentRoleClaimsValues).ToList();

                foreach (var permission in allPermissions)
                {
                    if (currentlyAssignedRoleClaimsNames.Any(carc => carc == permission.Name))
                    {
                        roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                        {
                            RoleId = roleId,
                            ClaimType = AppClaim.Permission,
                            ClaimValue = permission.Name,
                            Description = permission.Description,
                            Group = permission.Group,
                            IsAssignedToRole = true
                        });
                    }
                    else
                    {
                        roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                        {
                            RoleId = roleId,
                            ClaimType = AppClaim.Permission,
                            ClaimValue = permission.Name,
                            Description = permission.Description,
                            Group = permission.Group,
                            IsAssignedToRole = false
                        });
                    }
                }
                return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
            }
            return await ResponseWrapper<RoleClaimResponse>.FailAsync("Role does not exist");
        }

        public async Task<IResponseWrapper> UpdateRolePermissionAsync(UpdateRolePermissionsRequest request)
        {
            var roleInDb = await _roleManager.FindByIdAsync(request.RoleId);
            if (roleInDb is not null)
            {
                if(roleInDb.Name == AppRoles.Admin)
                {
                    return await ResponseWrapper<string>.FailAsync("Admin role permissions cannot be updated");
                }

                var permissionToBeAssigned = request.RoleClaims.Where(rc => rc.IsAssignedToRole==true).ToList();
                var currentlyAssignedClaims = await _roleManager.GetClaimsAsync(roleInDb);

                //Remove
                foreach (var claim in currentlyAssignedClaims)
                {
                    await _roleManager.RemoveClaimAsync(roleInDb, claim);
                }

                //Add
                foreach (var claim in permissionToBeAssigned)
                {
                    var mappedRoleClaim = _mapper.Map<ApplicationRoleClaim>(claim);
                    await _context.RoleClaims.AddAsync(mappedRoleClaim);
                    await _context.SaveChangesAsync();
                   
                }

                return await ResponseWrapper<string>.SuccessAsync("Role permissions updated successfully");
            }
            return await ResponseWrapper<string>.FailAsync("Role does not exist");
        }

        private async Task<List<RoleClaimViewModel>> GetAllClaimsForRoleAsync(string roleId)
        {
            var roleClaims = await _context.RoleClaims.Where(rc => rc.RoleId == roleId).ToListAsync();
          

            if (roleClaims.Count > 0)
            {
                var mappedRoleClaims = _mapper.Map<List<RoleClaimViewModel>>(roleClaims);
                return mappedRoleClaims;
            }

            return new List<RoleClaimViewModel>();

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

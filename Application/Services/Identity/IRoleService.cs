using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity
{
    public interface IRoleService
    {
        Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request);
        Task<IResponseWrapper> GetRolesAsync();
        Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request);
        Task<IResponseWrapper> GetRoleByIdAsync(string roleId);
        Task<IResponseWrapper> DeleteRoleAsync(string roleId);
        Task<IResponseWrapper> GetPermissionAsync(string roleId);
        Task<IResponseWrapper> UpdateRolePermissionAsync(UpdateRolePermissionsRequest request);
    }
}

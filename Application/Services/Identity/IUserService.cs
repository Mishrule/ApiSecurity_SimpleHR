using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity
{
	public interface IUserService
	{
		Task<IResponseWrapper> RegisterUser (UserRegistrationRequest userRegistrationRequest);
		Task<IResponseWrapper> GetUserById(string userId);
		Task<IResponseWrapper> GetAllUsersAsync();
		Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request);
		Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request);
		Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request);
		Task<IResponseWrapper> GetRoleAsync(string userId);
		Task<IResponseWrapper> UpdateUserRoleAsync(UpdateUserRolesRequest request);
	}
}

using Application.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly IMapper _mapper;

		public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
			IMapper mapper)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
		}

		public async Task<IResponseWrapper> RegisterUser(UserRegistrationRequest request)
		{
			var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
			if (userWithSameEmail is not null)
			{
				return await ResponseWrapper.FailAsync("User already exists");
			}

			var userWithSameUsername = await _userManager.FindByNameAsync(request.UserName);
			if (userWithSameUsername is not null)
			{
				return await ResponseWrapper.FailAsync("Username already exists");
			}

			var newUser = new ApplicationUser
			{
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email,
				UserName = request.UserName,
				PhoneNumber = request.PhoneNumber,
				IsActive = request.ActivateUser,
				EmailConfirmed = request.AutoComfirmEmail
			};

			var password = new PasswordHasher<ApplicationUser>();
			newUser.PasswordHash = password.HashPassword(newUser, request.Password);



			var identityResult = await _userManager.CreateAsync(newUser);
			if (identityResult.Succeeded)
			{
				//Assign to Role
				await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);
				return await ResponseWrapper<string>.SuccessAsync("User registered Successfully");
			}

			return await ResponseWrapper.FailAsync("User registration failed");
		}

		public async Task<IResponseWrapper> GetUserById(string userId)
		{
			var userInDb = await _userManager.FindByIdAsync(userId);
			if (userInDb is not null)
			{
				var mappedUser = _mapper.Map<UserResponse>(userInDb);
				return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
			}

			return await ResponseWrapper.FailAsync("User not found");
		}

		public async Task<IResponseWrapper> GetAllUsersAsync()
		{
			var usersInDb = await _userManager.Users.ToListAsync();
			if (usersInDb is not null)
			{
				var mappedUsers = _mapper.Map<List<UserResponse>>(usersInDb);
				return await ResponseWrapper<List<UserResponse>>.SuccessAsync(mappedUsers);
			}

			return await ResponseWrapper.FailAsync("No users found");
		}

		public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request)
		{
			var userInDb = await _userManager.FindByIdAsync(request.UserId);
			if (userInDb is not null)
			{
				userInDb.FirstName = request.FirstName;
				userInDb.LastName = request.LastName;
				userInDb.PhoneNumber = request.PhoneNumber;

				var identityResult = await _userManager.UpdateAsync(userInDb);
				if (identityResult.Succeeded)
				{
					return await ResponseWrapper<string>.SuccessAsync("User updated successfully");
				}

				return await ResponseWrapper.FailAsync("User update failed");
			}

			return await ResponseWrapper.FailAsync("User does not exist");
		}

		public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request)
		{
			var userInDb = await _userManager.FindByIdAsync(request.UserId);
			if (userInDb is not null)
			{
				var identityResult = await _userManager.ChangePasswordAsync(userInDb, request.CurrentPassword, request.NewPassword);
				if(identityResult.Succeeded)
				{
					return await ResponseWrapper<string>.SuccessAsync("Password changed successfully");
				}
				return await ResponseWrapper.FailAsync("Password change failed");
			}
			return await ResponseWrapper.FailAsync("User does not exist");
		}

		public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request)
		{
			var userInDb = await _userManager.FindByIdAsync(request.UserId);

			if (userInDb is not null)
			{
				userInDb.IsActive = request.Activate;
				var identityResult = await _userManager.UpdateAsync(userInDb);
				if (identityResult.Succeeded)
				{
					return await ResponseWrapper<string>.SuccessAsync(request.Activate?"User status updated successfully":"User de-activated successfully");
				}

				return await ResponseWrapper.FailAsync(request.Activate?"Failed to activate user":"Failed to de-activate user");
			}
			return await ResponseWrapper.FailAsync("User does not exist");
		}
	}
}

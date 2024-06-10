using Application.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<IResponseWrapper> RegisterUser(UserRegistrationRequest request)
		{
			var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
			if (userWithSameEmail is not null)
			{
				return await ResponseWrapper.FailAsync("User already exists");
			}

			var userWithSameUsername = await _userManager.FindByNameAsync(request.UserName);
			if(userWithSameUsername is not null)
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

			var identityResult = await _userManager.CreateAsync(newUser);
			if(identityResult.Succeeded)
			{
				//Assign to Role
				await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);
				return await ResponseWrapper<string>.SuccessAsync("User registered Successfully");
			}

			return await ResponseWrapper.FailAsync("User registration failed");
		}
	}
}

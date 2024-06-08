﻿using Application.Services.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.AppConfigs;
using Common.Requests;
using Common.Responses;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Identity
{
	public class TokenService : ITokenService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly AppConfiguration _appConfiguration;

		public TokenService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
			IOptions<AppConfiguration> appConfiguration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_appConfiguration = appConfiguration.Value;
		}

		public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
		{
			//validate user
			//check user
			var user = await _userManager.FindByEmailAsync(tokenRequest.Email);
			if (user is null)
			{
				return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials");
			}

			//Check if User is active
			if (!user.IsActive)
			{
				return await ResponseWrapper<TokenResponse>.FailAsync("User is not active, please contact Administrator");
			}
			//Check email is confirmed
			if (!user.EmailConfirmed)
			{
				return await ResponseWrapper<TokenResponse>.FailAsync("Email is not confirmed, please confirm your email");
			}
			//Check Password
			var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);
			if (!isPasswordValid)
			{
				return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials");
			}
			//Generate referenceToken
			user.RefreshToken = GenerateReferenceToken();
			user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);
			//Update user
			await _userManager.UpdateAsync(user);

			//Generate new token
			var token = await GenerateJWTAsync(user);
			//return
			var response = new TokenResponse
			{
				Token = token,
				RefreshToken = user.RefreshToken,
				RefreshTokenExpireTime = user.RefreshTokenExpireDate
			};

			return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
		}

		public async Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refereshTokenRequest)
		{
			if (refereshTokenRequest is null)
			{
				return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token");
			}

			var userPrincipal = GetPrincipalFromExpiredToken(refereshTokenRequest.Token);
			var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.FindByEmailAsync(userEmail);

			if(user is null)
				return await ResponseWrapper<TokenResponse>.FailAsync("User Not found");
			if(user.RefreshToken != refereshTokenRequest.RefreshToken || user.RefreshTokenExpireDate <= DateTime.UtcNow)
				return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Refresh Token");

			var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
			user.RefreshToken = GenerateReferenceToken();
			await _userManager.UpdateAsync(user);

			var response = new TokenResponse
			{
				Token = token,
				RefreshToken = user.RefreshToken,
				RefreshTokenExpireTime = user.RefreshTokenExpireDate
			};

			return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
		}

		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
				ValidateIssuer = false,
				ValidateAudience = false,
				RoleClaimType = ClaimTypes.Role,
				ClockSkew = TimeSpan.Zero
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
			if (securityToken is not JwtSecurityToken jwtSecurityToken ||
			    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
				    StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}

		private string GenerateReferenceToken()
		{
			var randomNumber = new byte[32];
			using var rnd = RandomNumberGenerator.Create();
			rnd.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		private async Task<string> GenerateJWTAsync(ApplicationUser user)
		{
			var token =  GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
			return token;
		}


		private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
		{
			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpireInMinutes),
				signingCredentials: signingCredentials);

			var tokenHandler = new JwtSecurityTokenHandler();
			var encryptedToken = tokenHandler.WriteToken(token);

			return encryptedToken;
		}

		private SigningCredentials GetSigningCredentials()
		{
			var key = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
			var secret = new SymmetricSecurityKey(key);
			return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
		}

		private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
		{
			var userClaims = await _userManager.GetClaimsAsync(user);
			var roles = await _userManager.GetRolesAsync(user);
			var roleClaims = new List<Claim>();
			var permissionClaims = new List<Claim>();

			foreach (var role in roles)
			{
				roleClaims.Add(new Claim(ClaimTypes.Role, role));
				var currentRole = await _roleManager.FindByNameAsync(role);
				var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
				permissionClaims.AddRange(allPermissionsForCurrentRole);
			}


			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.NameIdentifier, user.Id),
					new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? String.Empty),
					new Claim(ClaimTypes.Surname, user.LastName)
				}

				.Union(userClaims)
				.Union(roleClaims)
				.Union(permissionClaims);
			
			return claims;
		}
	}
}

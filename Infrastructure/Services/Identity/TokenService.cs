using Application.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Requests;
using Common.Responses;

namespace Infrastructure.Services.Identity
{
	public class TokenService : ITokenService
	{
		public async Task<TokenResponse> GetTokenAsync(TokenRequest request)
		{
			throw new NotImplementedException();
		}

		public async Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest request)
		{
			throw new NotImplementedException();
		}
	}
}

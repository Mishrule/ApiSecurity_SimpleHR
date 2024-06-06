using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Requests;
using Common.Responses;

namespace Application.Services.Identity
{
    public interface ITokenService
	{
		Task<TokenResponse> GetTokenAsync(TokenRequest request);
		Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest request);
	}
}

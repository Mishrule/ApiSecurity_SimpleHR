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
	}
}

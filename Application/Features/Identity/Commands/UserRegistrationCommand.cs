using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Commands
{
    public class UserRegistrationCommand : IRequest<IResponseWrapper>
    {
        public UserRegistrationRequest UserRegistration { get; set; }
    }

    public class UserRegistrationCommandHandler : IRequestHandler<UserRegistrationCommand, IResponseWrapper>
	{
		public Task<IResponseWrapper> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}

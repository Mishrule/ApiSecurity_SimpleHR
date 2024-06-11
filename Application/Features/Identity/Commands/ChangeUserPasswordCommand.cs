﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Commands
{
	public class ChangeUserPasswordCommand : IRequest<IResponseWrapper>
	{
		public ChangePasswordRequest ChangePassword { get; set; }
	}

	public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IResponseWrapper>
	{private readonly IUserService _userService;

		public ChangeUserPasswordCommandHandler(IUserService userService)
		{
			_userService = userService;
		}

		public async Task<IResponseWrapper> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
		{
			return await _userService.ChangeUserPasswordAsync(request.ChangePassword);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Identity.Users.Commands;
using Application.Services.Identity;
using Common.Requests.Identity;
using FluentValidation;

namespace Application.Features.Employees.Validators
{
    public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
    {
        public UserRegistrationCommandValidator(IUserService userService)
        {
            RuleFor(x => x.UserRegistration).SetValidator(new UserRegistrationRequestValidator(userService));
        }
    }
}

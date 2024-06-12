using Common.Requests.Employees;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Identity;

namespace Application.Features.Employees.Validators
{
    public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator(IUserService userService)
        {
            RuleFor(request => request.UserName)
                .MustAsync(async (email, ct)=>await userService.GetUserByEmailAsync(email) is not UserResponse existingUser).WithMessage("Email already in use");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");

            RuleFor(request => request.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(request=>request.ConfirmPassword).Must((request, confirmPassword) => request.Password == confirmPassword).WithMessage("Passwords do not match");



        }
    }
}

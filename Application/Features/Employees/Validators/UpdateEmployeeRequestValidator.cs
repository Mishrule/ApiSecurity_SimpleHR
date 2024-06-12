using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services;
using Common.Requests.Employees;
using Domain;
using FluentValidation;

namespace Application.Features.Employees.Validator
{
    public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
    {
        public UpdateEmployeeRequestValidator(IEmployeeService employeeService)
        {
            RuleFor(request => request.Id).MustAsync(async (id, ct) =>
                    await employeeService.GetEmployeeByIdAsync(id) is Employee employeeinDb && employeeinDb.Id == id)
                .WithMessage("Employee does not exit.");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email is not valid.");
            RuleFor(x => x.Salary).NotEmpty().WithMessage("Email is not valid.");
            
        }
    }
}

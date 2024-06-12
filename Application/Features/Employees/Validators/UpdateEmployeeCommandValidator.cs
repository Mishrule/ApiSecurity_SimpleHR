using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Employees.Commands;
using Application.Services;
using FluentValidation;

namespace Application.Features.Employees.Validator
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator(IEmployeeService employeeService)
        {
            RuleFor(command => command.UpdateEmployeeRequest).SetValidator(new UpdateEmployeeRequestValidator(employeeService));
        }
    }
}

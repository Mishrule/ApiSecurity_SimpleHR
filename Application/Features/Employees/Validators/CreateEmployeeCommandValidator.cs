using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Employees.Commands;
using FluentValidation;

namespace Application.Features.Employees.Validator
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(command => command.CreateEmployeeRequest).SetValidator(new CreateEmployeeRequestValidator());
        }
    }
    
}

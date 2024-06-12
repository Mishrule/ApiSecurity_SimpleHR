using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Pipelines;
using Application.Services;
using AutoMapper;
using Common.Requests.Employees;
using Common.Responses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Commands
{
    public class UpdateEmployeeCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public UpdateEmployeeRequest UpdateEmployeeRequest { get; set; } 
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public UpdateEmployeeCommandHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {

            var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.UpdateEmployeeRequest.Id);
            if (employeeInDb is not null)
            {
                employeeInDb.FirstName = request.UpdateEmployeeRequest.FirstName;
                employeeInDb.LastName = request.UpdateEmployeeRequest.LastName;
                employeeInDb.Email = request.UpdateEmployeeRequest.Email;
                employeeInDb.Salary = request.UpdateEmployeeRequest.Salary;

                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeInDb);

                var mappedEmployeeResponse = _mapper.Map<EmployeesResponse>(updatedEmployee);
                return await ResponseWrapper<EmployeesResponse>.SuccessAsync(mappedEmployeeResponse, "Employee updated successfully");

            }

            return await ResponseWrapper.FailAsync("Employee not found");

        }
    }
}

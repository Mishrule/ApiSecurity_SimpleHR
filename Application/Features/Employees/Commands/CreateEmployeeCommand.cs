﻿using System;
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
using Domain;
using MediatR;

namespace Application.Features.Employees.Commands
{
    public class CreateEmployeeCommand : IRequest<IResponseWrapper>, IValidateMe
    {
      public  CreateEmployeeRequest CreateEmployeeRequest { get; set; }
    }

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public CreateEmployeeCommandHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var mappedEmployee = _mapper.Map<Employee>(request.CreateEmployeeRequest);
            var newEmployee = await _employeeService.CreateEmployeeAsync(mappedEmployee);
            if(newEmployee.Id > 0)
            {
                var mappedEmployeeResponse = _mapper.Map<EmployeesResponse>(newEmployee);
                return await ResponseWrapper<EmployeesResponse>.SuccessAsync(mappedEmployeeResponse, "Employee created successfully");
            }

            return await ResponseWrapper.FailAsync("Failed to Create employee entry");
        }
    }
}

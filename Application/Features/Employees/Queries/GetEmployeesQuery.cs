using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Application.Services;
using AutoMapper;
using Common.Responses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Queries
{
    public class GetEmployeesQuery : IRequest<IResponseWrapper>
    {
    }


    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public GetEmployeesQueryHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employeeList = await _employeeService.GetEmployeeListAsync();
            if(employeeList.Count > 0)
            {
                var mappedEmployeeList = _mapper.Map<List<EmployeesResponse>>(employeeList);
                return await ResponseWrapper<List<EmployeesResponse>>.SuccessAsync(mappedEmployeeList, "Employee list retrieved successfully");
            }
            return await ResponseWrapper.FailAsync("No employee found");
        }
    }
}


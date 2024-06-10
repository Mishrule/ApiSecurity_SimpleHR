using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services;
using AutoMapper;
using Common.Responses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Queries
{
    public class GetEmployeeByIdQuery : IRequest<IResponseWrapper>
    {
        public GetEmployeeByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }


    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public GetEmployeeByIdQueryHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(request.Id);
            if (employee is not null)
            {
                var mappedEmployeeList =  _mapper.Map<EmployeesResponse>(employee);
                return await ResponseWrapper<EmployeesResponse>.SuccessAsync(mappedEmployeeList, "Employee found successfully");
            }

            return await ResponseWrapper.FailAsync("Employee not found");
        }
    }
}

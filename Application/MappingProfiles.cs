using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.Requests.Employees;
using Common.Responses;
using Common.Responses.Identity;
using Domain;

namespace Application
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<CreateEmployeeRequest, Employee>().ReverseMap();
			CreateMap<Employee, EmployeesResponse>().ReverseMap();
			
		}
	}
}

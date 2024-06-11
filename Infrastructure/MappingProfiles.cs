using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.Responses.Identity;
using Infrastructure.Models;

namespace Infrastructure
{
   internal class MappingProfiles : Profile
    {
	    public MappingProfiles()
	    {
		    CreateMap<ApplicationUser, UserResponse>().ReverseMap();
		    CreateMap<ApplicationRole, RoleResponse>().ReverseMap();
		    CreateMap<ApplicationRoleClaim, RoleClaimViewModel>().ReverseMap();
		}
    }
}

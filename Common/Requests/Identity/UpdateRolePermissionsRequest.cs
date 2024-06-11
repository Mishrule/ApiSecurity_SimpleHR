using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Responses.Identity;

namespace Common.Requests.Identity
{
	public class UpdateRolePermissionsRequest
	{
        public string RoleId { get; set; }
        public List<RoleClaimViewModel> RoleClaims { get; set; }
    }
}

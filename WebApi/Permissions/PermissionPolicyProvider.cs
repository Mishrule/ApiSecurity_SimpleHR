

using Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace WebApi.Permissions
{
	public class PermissionPolicyProvider : IAuthorizationPolicyProvider
	{
		public DefaultAuthorizationPolicyProvider FallBackPolicyProvider { get; }

		public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
		{
			FallBackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		}

		public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
		{
			if (policyName.StartsWith(AppClaim.Permission, StringComparison.CurrentCultureIgnoreCase))
			{
				var policy = new AuthorizationPolicyBuilder();
				policy.AddRequirements(new PermissionRequirement(policyName));
				return Task.FromResult(policy.Build());
			}
			return FallBackPolicyProvider.GetPolicyAsync(policyName);
		}

		public  Task<AuthorizationPolicy> GetDefaultPolicyAsync()
		=> FallBackPolicyProvider.GetDefaultPolicyAsync();
		 
			

		public  Task<AuthorizationPolicy> GetFallbackPolicyAsync()
		=>  Task.FromResult<AuthorizationPolicy>(null);
	}
	
}

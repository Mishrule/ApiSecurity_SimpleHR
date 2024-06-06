using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Authorization
{
	public record AppPermission(string Feature, string Action, string Description, string Group, bool IsBasic = false)
	{
		public string Name => NameFor(Feature, Action);

		public static string NameFor(string feature, string action) => $"Permissions.{feature}.{action}";
	}

	public class AppPermissions
	{
		private static readonly AppPermission[] _all = new AppPermission[]
		{
			new(AppFeature.Users, AppAction.Create, AppRoleGroup.SystemAccess, "Create Users"),
			new(AppFeature.Users, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users"),
			new(AppFeature.Users, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users"),
			new(AppFeature.Users, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Users"),


			new(AppFeature.UserRoles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users Role"),
			new(AppFeature.UserRoles, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users Role"),
			
			new(AppFeature.Roles, AppAction.Create, AppRoleGroup.SystemAccess, "Create Role"),
			new(AppFeature.Roles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Role"),
			new(AppFeature.Roles, AppAction.Read, AppRoleGroup.SystemAccess, "Read Role"),
			new(AppFeature.Roles, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Role"),

			new(AppFeature.RoleClaims, AppAction.Update, AppRoleGroup.SystemAccess, "Update Role Claims/Permissions"),
			new(AppFeature.RoleClaims, AppAction.Read, AppRoleGroup.SystemAccess, "Read Role Claims/Permissions"),


			new(AppFeature.Employees, AppAction.Create, AppRoleGroup.SystemAccess, "Create Employees"),
			new(AppFeature.Employees, AppAction.Update, AppRoleGroup.SystemAccess, "Update Employees"),
			new(AppFeature.Employees, AppAction.Read, AppRoleGroup.SystemAccess, "Read Employees", IsBasic: true),
			new(AppFeature.Employees, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Employees"),
		};

		public static IReadOnlyList<AppPermission> AdminPermissions { get; }= new ReadOnlyCollection<AppPermission>(_all.Where(p=>!p.IsBasic).ToArray());
		public static IReadOnlyList<AppPermission> BasicPermissions { get; }= new ReadOnlyCollection<AppPermission>(_all.Where(p=>p.IsBasic).ToArray());
	}
}

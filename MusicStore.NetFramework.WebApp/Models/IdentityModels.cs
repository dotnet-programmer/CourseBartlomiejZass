using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MusicStore.NetFramework.WebApp.Models
{
	public class ApplicationUser : IdentityUser
	{
		public virtual ICollection<Order> Orders { get; set; }
		public UserData UserData { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}

	//public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	//{
	//    public ApplicationDbContext()
	//        : base("DefaultConnection", throwIfV1Schema: false)
	//    {
	//    }

	//    static ApplicationDbContext()
	//    {
	//        // Set the database intializer which is run once during application start
	//        // This seeds the database with admin user credentials and admin role
	//        Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
	//    }

	//    public static ApplicationDbContext Create()
	//    {
	//        return new ApplicationDbContext();
	//    }
	//}
}
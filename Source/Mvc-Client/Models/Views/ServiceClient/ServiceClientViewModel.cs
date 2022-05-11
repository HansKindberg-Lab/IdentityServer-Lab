using System.Security.Claims;

namespace MvcClient.Models.Views.ServiceClient
{
	public class ServiceClientViewModel
	{
		#region Properties

		public virtual IList<Claim> Claims { get; } = new List<Claim>();
		public virtual Exception Exception { get; set; }
		public virtual RequestMode? Mode { get; set; }
		public virtual string Token { get; set; }

		#endregion
	}
}
using System.Security.Claims;
using MvcClient.Models.IdentityModel.Tokens.Jwt;

namespace MvcClient.Models.Views.Account
{
	public class AccountViewModel
	{
		#region Properties

		public virtual IList<Claim> Claims { get; } = new List<Claim>();
		public virtual IDictionary<string, string> Properties { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		public virtual IDictionary<string, ITokenInformation> Tokens { get; } = new Dictionary<string, ITokenInformation>(StringComparer.OrdinalIgnoreCase);

		#endregion
	}
}
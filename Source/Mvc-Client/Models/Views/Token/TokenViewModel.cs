using MvcClient.Models.IdentityModel.Tokens.Jwt;

namespace MvcClient.Models.Views.Token
{
	public class TokenViewModel
	{
		#region Properties

		public virtual TokenMode? Mode { get; set; }
		public virtual ITokenInformation Token { get; set; }

		#endregion
	}
}
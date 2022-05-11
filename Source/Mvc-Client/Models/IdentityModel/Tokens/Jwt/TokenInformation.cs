namespace MvcClient.Models.IdentityModel.Tokens.Jwt
{
	public class TokenInformation : ITokenInformation
	{
		#region Properties

		public virtual string Encoded { get; set; }
		public virtual Exception Exception { get; set; }
		public virtual string Header { get; set; }
		public virtual DateTimeOffset? IssuedAt { get; set; }
		public virtual string Payload { get; set; }
		public virtual DateTimeOffset? ValidFrom { get; set; }
		public virtual DateTimeOffset? ValidTo { get; set; }

		#endregion
	}
}
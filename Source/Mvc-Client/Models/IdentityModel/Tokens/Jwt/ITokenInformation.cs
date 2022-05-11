namespace MvcClient.Models.IdentityModel.Tokens.Jwt
{
	public interface ITokenInformation
	{
		#region Properties

		string Encoded { get; }
		Exception Exception { get; }
		string Header { get; }
		DateTimeOffset? IssuedAt { get; }
		string Payload { get; }
		DateTimeOffset? ValidFrom { get; }
		DateTimeOffset? ValidTo { get; }

		#endregion
	}
}
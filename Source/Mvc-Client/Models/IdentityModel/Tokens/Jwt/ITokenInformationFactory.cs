namespace MvcClient.Models.IdentityModel.Tokens.Jwt
{
	public interface ITokenInformationFactory
	{
		#region Methods

		ITokenInformation Create(string encoded);

		#endregion
	}
}
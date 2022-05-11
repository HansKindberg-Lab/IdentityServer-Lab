using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MvcClient.Models.IdentityModel.Tokens.Jwt
{
	public class TokenInformationFactory : ITokenInformationFactory
	{
		#region Methods

		public virtual ITokenInformation Create(string encoded)
		{
			if(encoded == null)
				return null;

			var tokenInformation = new TokenInformation
			{
				Encoded = encoded
			};

			try
			{
				var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

				if(!jwtSecurityTokenHandler.CanReadToken(encoded))
					throw new InvalidOperationException("Can not read the token.");

				var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(encoded);

				tokenInformation.Header = this.DictionaryToJson(jwtSecurityToken.Header);

				if(jwtSecurityToken.IssuedAt > DateTime.MinValue)
					tokenInformation.IssuedAt = new DateTimeOffset(jwtSecurityToken.IssuedAt);

				if(jwtSecurityToken.ValidFrom > DateTime.MinValue)
					tokenInformation.ValidFrom = new DateTimeOffset(jwtSecurityToken.ValidFrom);

				if(jwtSecurityToken.ValidTo > DateTime.MinValue)
					tokenInformation.ValidTo = new DateTimeOffset(jwtSecurityToken.ValidTo);

				tokenInformation.Payload = this.DictionaryToJson(jwtSecurityToken.Payload);
			}
			catch(Exception exception)
			{
				tokenInformation.Exception = exception;
			}

			return tokenInformation;
		}

		protected internal virtual JsonSerializerOptions CreateJsonSerializerOptions()
		{
			return new JsonSerializerOptions
			{
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				WriteIndented = true
			};
		}

		protected internal virtual string DictionaryToJson(IDictionary<string, object> dictionary)
		{
			var resolvedDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			foreach(var (key, value) in dictionary.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
			{
				var resolvedValue = value;

				if(value is not string && value is IEnumerable enumerable)
					resolvedValue = enumerable.Cast<object>().Select(item => item.ToString()).ToArray();

				resolvedDictionary.Add(key, resolvedValue);
			}

			return JsonSerializer.Serialize(resolvedDictionary, this.CreateJsonSerializerOptions());
		}

		#endregion
	}
}
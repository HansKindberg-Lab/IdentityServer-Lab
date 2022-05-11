using IdentityModel;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models.IdentityModel.Tokens.Jwt;
using MvcClient.Models.Views.Token;

namespace MvcClient.Controllers
{
	public class TokenController : SiteController
	{
		#region Constructors

		public TokenController(IClientAccessTokenManagementService clientAccessTokenManagementService, ITokenInformationFactory tokenInformationFactory, ILoggerFactory loggerFactory, IUserAccessTokenManagementService userAccessTokenManagementService) : base(loggerFactory)
		{
			this.ClientAccessTokenManagementService = clientAccessTokenManagementService ?? throw new ArgumentNullException(nameof(clientAccessTokenManagementService));
			this.TokenInformationFactory = tokenInformationFactory ?? throw new ArgumentNullException(nameof(tokenInformationFactory));
			this.UserAccessTokenManagementService = userAccessTokenManagementService ?? throw new ArgumentNullException(nameof(userAccessTokenManagementService));
		}

		#endregion

		#region Properties

		protected internal virtual IClientAccessTokenManagementService ClientAccessTokenManagementService { get; }
		protected internal virtual ITokenInformationFactory TokenInformationFactory { get; }
		protected internal virtual IUserAccessTokenManagementService UserAccessTokenManagementService { get; }

		#endregion

		#region Methods

		protected internal virtual async Task<string> GetTokenAsync(TokenMode mode)
		{
			return mode switch
			{
				TokenMode.ManagedClientAccessToken => await this.ClientAccessTokenManagementService.GetClientAccessTokenAsync(),
				TokenMode.ManagedUserAccessToken => await this.UserAccessTokenManagementService.GetUserAccessTokenAsync(this.User),
				_ => await this.HttpContext.GetTokenAsync(OidcConstants.TokenTypes.AccessToken)
			};
		}

		public virtual async Task<IActionResult> Index(TokenMode? mode)
		{
			var model = new TokenViewModel
			{
				Mode = mode
			};

			// ReSharper disable InvertIf
			if(mode != null)
			{
				var token = await this.GetTokenAsync(mode.Value);

				model.Token = this.TokenInformationFactory.Create(token);
			}
			// ReSharper restore InvertIf

			return this.View(model);
		}

		#endregion
	}
}
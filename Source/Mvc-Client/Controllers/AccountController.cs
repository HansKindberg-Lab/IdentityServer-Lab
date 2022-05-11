using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models.IdentityModel.Tokens.Jwt;
using MvcClient.Models.Views.Account;

namespace MvcClient.Controllers
{
	[Authorize]
	public class AccountController : SiteController
	{
		#region Constructors

		public AccountController(ILoggerFactory loggerFactory, ITokenInformationFactory tokenInformationFactory) : base(loggerFactory)
		{
			this.TokenInformationFactory = tokenInformationFactory ?? throw new ArgumentNullException(nameof(tokenInformationFactory));
		}

		#endregion

		#region Properties

		protected internal virtual ITokenInformationFactory TokenInformationFactory { get; }

		#endregion

		#region Methods

		[AllowAnonymous]
		public virtual async Task<IActionResult> AccessDenied()
		{
			return await Task.FromResult(this.View());
		}

		public virtual async Task<IActionResult> Index()
		{
			var model = new AccountViewModel();

			var authenticateResult = await this.HttpContext.AuthenticateAsync();

			// ReSharper disable PossibleNullReferenceException

			foreach(var claim in authenticateResult.Principal.Claims.OrderBy(claim => claim.Type, StringComparer.OrdinalIgnoreCase))
			{
				model.Claims.Add(claim);
			}

			foreach(var (key, value) in authenticateResult.Properties.Items.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
			{
				model.Properties.Add(key, value);
			}

			// ReSharper restore PossibleNullReferenceException

			foreach(var tokenName in new[] { OidcConstants.TokenTypes.AccessToken, OidcConstants.TokenTypes.IdentityToken })
			{
				var encoded = authenticateResult.Properties.GetTokenValue(tokenName);

				if(encoded == null)
					continue;

				model.Tokens.Add(tokenName, this.TokenInformationFactory.Create(encoded));
			}

			return await Task.FromResult(this.View(model));
		}

		[AllowAnonymous]
		public virtual async Task<IActionResult> SignedOut(bool local)
		{
			if(this.User is { Identity.IsAuthenticated: true })
				return this.Redirect("/");

			return await Task.FromResult(this.View(new SignedOutViewModel { Local = local }));
		}

		[AllowAnonymous]
		public virtual async Task<IActionResult> SignIn(string returnUrl)
		{
			return await Task.FromResult(this.Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, OpenIdConnectDefaults.AuthenticationScheme));
		}

		public virtual async Task<IActionResult> SignOut(bool local)
		{
			var model = new SignOutViewModel
			{
				Form =
				{
					Local = local
				}
			};
			return await Task.FromResult(this.View(model));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> SignOut(SignOutForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			var authenticationSchemes = new List<string>
			{
				CookieAuthenticationDefaults.AuthenticationScheme
			};

			if(!form.Local)
				authenticationSchemes.Add(OpenIdConnectDefaults.AuthenticationScheme);

			//var redirectUri = form.Local ? this.Url.Action("SignedOut", new { Local = true }) : this.Url.Action("SignedOut");
			var redirectUri = this.Url.Action("SignedOut", form.Local ? new { Local = true } : null);

			if(!form.Local)
				return await Task.FromResult(this.SignOut(new AuthenticationProperties { RedirectUri = redirectUri }, authenticationSchemes.ToArray()));

			await this.HttpContext.SignOutAsync(authenticationSchemes.First());

			// ReSharper disable AssignNullToNotNullAttribute
			return this.Redirect(redirectUri);
			// ReSharper restore AssignNullToNotNullAttribute
		}

		#endregion
	}
}
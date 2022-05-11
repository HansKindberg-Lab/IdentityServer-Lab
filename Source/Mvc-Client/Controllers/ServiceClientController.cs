using System.Security.Claims;
using Grpc.Core;
using Grpc.Net.Client;
using IdentityModel;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models.Views.ServiceClient;
using Service.Definitions;

namespace MvcClient.Controllers
{
	public class ServiceClientController : SiteController
	{
		#region Constructors

		public ServiceClientController(IClientAccessTokenManagementService clientAccessTokenManagementService, ILoggerFactory loggerFactory, IUserAccessTokenManagementService userAccessTokenManagementService) : base(loggerFactory)
		{
			this.ClientAccessTokenManagementService = clientAccessTokenManagementService ?? throw new ArgumentNullException(nameof(clientAccessTokenManagementService));
			this.UserAccessTokenManagementService = userAccessTokenManagementService ?? throw new ArgumentNullException(nameof(userAccessTokenManagementService));
		}

		#endregion

		#region Properties

		protected internal virtual IClientAccessTokenManagementService ClientAccessTokenManagementService { get; }
		protected internal virtual IUserAccessTokenManagementService UserAccessTokenManagementService { get; }

		#endregion

		#region Methods

		protected internal virtual async Task<ClaimsResponse> GetServiceResponseAsync(string token)
		{
			using(var channel = GrpcChannel.ForAddress("https://localhost:5002"))
			{
				var client = new ClaimsService.ClaimsServiceClient(channel);

				var headers = new Metadata
				{
					{ "Authorization", $"Bearer {token}" }
				};

				var response = await client.GetAsync(new ClaimsRequest(), headers);

				return response;
			}
		}

		protected internal virtual async Task<string> GetTokenAsync(RequestMode mode)
		{
			return mode switch
			{
				RequestMode.ManagedClientAccessToken => await this.ClientAccessTokenManagementService.GetClientAccessTokenAsync(),
				RequestMode.ManagedUserAccessToken => await this.UserAccessTokenManagementService.GetUserAccessTokenAsync(this.User),
				RequestMode.UserAccessToken => await this.HttpContext.GetTokenAsync(OidcConstants.TokenTypes.AccessToken),
				_ => null
			};
		}

		public virtual async Task<IActionResult> Index(RequestMode? mode)
		{
			var model = new ServiceClientViewModel
			{
				Mode = mode
			};

			// ReSharper disable InvertIf
			if(mode != null)
			{
				try
				{
					model.Token = await this.GetTokenAsync(mode.Value);

					var response = await this.GetServiceResponseAsync(model.Token);

					foreach(var claim in response.Claims)
					{
						model.Claims.Add(new Claim(claim.Type, claim.Value));
					}
				}
				catch(Exception exception)
				{
					model.Exception = exception;
				}
			}
			// ReSharper restore InvertIf

			return this.View(model);
		}

		#endregion
	}
}
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MvcClient.Models.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITokenInformationFactory, TokenInformationFactory>();
builder.Services.AddAuthentication(options =>
	{
		options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
		options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	})
	.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddOpenIdConnect(options =>
	{
		options.Authority = "https://localhost:5001";
		options.ClaimActions.MapAllExcept(options.ClaimActions.OfType<DeleteClaimAction>().Select(deleteClaimAction => deleteClaimAction.ClaimType).ToArray());
		options.ClientId = "interactive";
		options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
		options.GetClaimsFromUserInfoEndpoint = true;
		options.MapInboundClaims = false;
		options.ResponseType = "code";
		options.SaveTokens = true;
		options.Scope.Clear();
		options.Scope.Add("offline_access");
		options.Scope.Add("openid");
		options.Scope.Add("profile");
		options.TokenValidationParameters = new TokenValidationParameters
		{
			NameClaimType = JwtClaimTypes.Name,
			RoleClaimType = JwtClaimTypes.Role,
		};
	});
builder.Services.AddAccessTokenManagement(options =>
{
	options.Client.Clients.Add("service-client", new ClientCredentialsTokenRequest
	{
		Address = "https://localhost:5001/connect/token",
		ClientId = "m2m.client",
		ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A"
	});
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();

app.Run();
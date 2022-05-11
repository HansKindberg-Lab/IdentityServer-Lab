using Microsoft.AspNetCore.Authentication.JwtBearer;
using Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		//options.Audience = "https://localhost:5001/resources";
		options.Authority = "https://localhost:5001";
		options.MapInboundClaims = false;
		options.TokenValidationParameters.ValidateAudience = false;
		options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
	});
builder.Services.AddAuthorization();
builder.Services.AddGrpc();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
	endpoints.MapGrpcService<ClaimsService>().RequireAuthorization();
});

app.Run();
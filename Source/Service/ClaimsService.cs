using Grpc.Core;
using Service.Definitions;

namespace Service
{
	public class ClaimsService : Service.Definitions.ClaimsService.ClaimsServiceBase
	{
		#region Constructors

		public ClaimsService(ILoggerFactory loggerFactory)
		{
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
		}

		#endregion

		#region Properties

		protected internal virtual ILogger Logger { get; }

		#endregion

		#region Methods

		public override async Task<ClaimsResponse> Get(ClaimsRequest request, ServerCallContext context)
		{
			if(request == null)
				throw new ArgumentNullException(nameof(request));

			if(context == null)
				throw new ArgumentNullException(nameof(context));

			var httpContext = context.GetHttpContext();
			var response = new ClaimsResponse();

			foreach(var claim in httpContext.User.Claims.OrderBy(claim => claim.Type, StringComparer.OrdinalIgnoreCase))
			{
				response.Claims.Add(new ClaimsResponse.Types.Claim { Type = claim.Type, Value = claim.Value });
			}

			return await Task.FromResult(response);
		}

		#endregion
	}
}
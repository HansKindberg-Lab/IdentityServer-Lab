using Microsoft.AspNetCore.Mvc.Razor;

namespace MvcClient.Models.Web.Mvc.Razor.Extensions
{
	public static class RazorPageExtension
	{
		#region Methods

		public static bool IsActive(this RazorPageBase razorPage, string action = "Index", string controller = "Home")
		{
			var routeValueDictionary = razorPage?.ViewContext.RouteData.Values;

			// ReSharper disable InvertIf
			if(routeValueDictionary != null)
			{
				if(routeValueDictionary.TryGetValue("action", out var actionValue) && routeValueDictionary.TryGetValue("controller", out var controllerValue))
				{
					if(actionValue is string actionStringValue && controllerValue is string controllerStringValue)
						return string.Equals(action, actionStringValue, StringComparison.OrdinalIgnoreCase) && string.Equals(controller, controllerStringValue, StringComparison.OrdinalIgnoreCase);
				}
			}
			// ReSharper restore InvertIf

			return false;
		}

		#endregion
	}
}
// Microsoft.AspNetCore.Authorization.AuthorizationMiddleware
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace YDH.Finace.Web.Middlewares
{
	public class AuthorizationMiddleware2
	{
		private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";

		private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

		private readonly RequestDelegate _next;

		private readonly IAuthorizationPolicyProvider _policyProvider;

		public AuthorizationMiddleware2(RequestDelegate next, IAuthorizationPolicyProvider policyProvider)
		{
			_next = (next ?? throw new ArgumentNullException(nameof(next)));
			_policyProvider = (policyProvider ?? throw new ArgumentNullException(nameof(policyProvider)));
		}

		public async Task Invoke(HttpContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			Endpoint endpoint = context.GetEndpoint();
			if (endpoint != null)
			{
				context.Items["__AuthorizationMiddlewareWithEndpointInvoked"] = AuthorizationMiddlewareWithEndpointInvokedValue;
			}
			IReadOnlyList<IAuthorizeData> authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
			AuthorizationPolicy policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
			if (policy == null)
			{
				await _next(context);
				return;
			}
			IPolicyEvaluator policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();
			AuthenticateResult authenticationResult = await policyEvaluator.AuthenticateAsync(policy, context);
			if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
			{
				await _next(context);
				return;
			}
			PolicyAuthorizationResult policyAuthorizationResult = await policyEvaluator.AuthorizeAsync(policy, authenticationResult, context, endpoint);
			if (policyAuthorizationResult.Challenged)
			{
				if (policy.AuthenticationSchemes.Any())
				{
					foreach (string authenticationScheme in policy.AuthenticationSchemes)
					{
						await context.ChallengeAsync(authenticationScheme);
					}
				}
				else
				{
					await context.ChallengeAsync();
				}
			}
			else if (policyAuthorizationResult.Forbidden)
			{
				if (policy.AuthenticationSchemes.Any())
				{
					foreach (string authenticationScheme2 in policy.AuthenticationSchemes)
					{
						await context.ForbidAsync(authenticationScheme2);
					}
				}
				else
				{
					await context.ForbidAsync();
				}
			}
			else
			{
				await _next(context);
			}
		}
	}
}

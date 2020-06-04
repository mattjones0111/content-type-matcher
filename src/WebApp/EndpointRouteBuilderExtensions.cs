namespace WebAppMapping
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public static class EndpointRouteBuilderExtensions
    {
        static readonly string[] AllowedMethods =
        {
            "GET", "POST", "PUT", "DELETE", "PATCH"
        };

        public static IEndpointConventionBuilder Map(
            this IEndpointRouteBuilder builder,
            string method,
            string pattern,
            RequestDelegate requestDelegate,
            string mediaType = default)
        {
            if (!AllowedMethods.Contains(method, StringComparer.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException(
                    $"{method} is not an allowed HTTP method.",
                    nameof(method));
            }

            IEndpointConventionBuilder result = null;

            if (string.Equals(method, "GET", StringComparison.InvariantCultureIgnoreCase))
            {
                // GETs don't have a body, and therefore no media type.
                return builder.MapGet(pattern, requestDelegate);
            }

            if (string.Equals(method, "POST", StringComparison.InvariantCultureIgnoreCase))
            {
                result = builder.MapPost(pattern, requestDelegate);
            }

            if (string.Equals(method, "PUT", StringComparison.InvariantCultureIgnoreCase))
            {
                result = builder.MapPut(pattern, requestDelegate);
            }

            if (string.Equals(method, "DELETE", StringComparison.InvariantCultureIgnoreCase))
            {
                result = builder.MapDelete(pattern, requestDelegate);
            }

            if (!string.IsNullOrEmpty(mediaType))
            {
                result = result.WithMetadata(new ContentTypeMetadata(mediaType));
            }

            return result;
        }
    }
}
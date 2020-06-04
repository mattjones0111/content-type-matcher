namespace WebAppMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Matching;

    public class PostedContentTypeMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        public override int Order => 1;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            return endpoints.Any(t => t.Metadata.GetMetadata<ContentTypeMetadata>() != default);
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            bool anyMatched = false;

            string contentTypeHeader = httpContext
                .Request
                .Headers["Content-Type"]
                .FirstOrDefault();

            for (int i = 0; i < candidates.Count; i++)
            {
                CandidateState state = candidates[i];

                IHttpMethodMetadata httpMethod = state
                    .Endpoint
                    .Metadata
                    .GetMetadata<IHttpMethodMetadata>();

                if (httpMethod.HttpMethods.Count == 1 &&
                    string.Equals("GET", httpMethod.HttpMethods[0], StringComparison.InvariantCultureIgnoreCase))
                {
                    candidates.SetValidity(i, false);
                    continue;
                }

                ContentTypeMetadata contentTypeMeta = state
                    .Endpoint
                    .Metadata
                    .GetMetadata<ContentTypeMetadata>();

                if (contentTypeMeta == default) continue;

                bool match = string.Equals(
                    contentTypeMeta.ContentType,
                    contentTypeHeader,
                    StringComparison.InvariantCultureIgnoreCase);

                if (!match)
                {
                    candidates.SetValidity(i, false);
                }

                if (match)
                {
                    anyMatched = true;
                }
            }

            if (!anyMatched)
            {
                httpContext.SetEndpoint(CreateRejectionEndpoint());
            }

            return Task.CompletedTask;
        }

        Endpoint CreateRejectionEndpoint()
        {
            return new Endpoint(
                context =>
                {
                    context.Response.StatusCode = 415;
                    return Task.CompletedTask;
                },
                EndpointMetadataCollection.Empty,
                "415 Unsupported Media Type");
        }
    }
}

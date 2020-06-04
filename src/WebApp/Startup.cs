namespace WebAppMapping
{
    using System.Net;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MatcherPolicy, PostedContentTypeMatcherPolicy>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map(
                    "GET",
                    "/hello",
                    async context =>
                    {
                        await context.Response.WriteAsync("Hello");
                    });

                endpoints.Map(
                    "POST",
                    "/non-content-type-endpoint",
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        await context.Response.WriteAsync("OK!");
                    });

                endpoints.Map(
                    "POST",
                    "/endpoint",
                    async context =>
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        await context.Response.WriteAsync("Widget 1");
                    },
                    "application/vnd.widget.v1+json");

                endpoints.Map(
                    "POST",
                    "/endpoint",
                    async context =>
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        await context.Response.WriteAsync("Widget 2");
                    },
                    "application/vnd.widget.v2+json");
            });
        }
    }
}

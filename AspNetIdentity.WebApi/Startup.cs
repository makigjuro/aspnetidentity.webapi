using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using AspNetIdentity.WebApi;
using AspNetIdentity.WebApi.Filters;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json.Serialization;
using Owin;

[assembly:OwinStartup(typeof(Startup))]
namespace AspNetIdentity.WebApi
{
	public partial class Startup
	{
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            RegisterFilters(httpConfig);
            RegisterFormatters(httpConfig);
            RegisterRoutes(httpConfig);

            app.UseCors(CorsOptions.AllowAll);

            app.UseWebApi(httpConfig);
        }

        private void RegisterFilters(HttpConfiguration config)
        {
            config.Filters.Add(new JwtAuthenticationFilter());
        }

        private void RegisterFormatters(HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }

        private void RegisterRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            IEnumerable<DelegatingHandler> handlers = config.DependencyResolver.GetServices(typeof(DelegatingHandler)).Cast<DelegatingHandler>();

            HttpMessageHandler routeHandlers = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), handlers);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: null,
                handler: routeHandlers
            );
        }
	}
}	
using System.Web.Http;
using System.Web.Http.Cors;

namespace API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Reemplaza "http://localhost:5000" con la URL y puerto DONDE SE EJECUTARÁ TU NUEVO PROYECTO FRONT-END

            var cors = new EnableCorsAttribute("*", "*", "*"); // Origenes, Encabezados, Métodos
            config.EnableCors(cors);

            // Configuración y servicios de Web API

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
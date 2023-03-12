using System.Web.Mvc;
using System.Web.Routing;

namespace SSR.Net.React17DotNetFramework
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            React17SSR.ConfigureReact17();
            React18SSR.ConfigureReact18();
            Vue3SSR.ConfigureVue3();
        }
    }
}

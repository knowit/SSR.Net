using System.Web.Mvc;
using System.Web.Routing;
using SSR.Net.React17DotNetFramework;
using SSR.Net.React18DotNetFramework;
using SSR.Net.React19DotNetFramework;
using SSR.Net.Vue3DotNetFramework;

namespace SSR.Net.DotNetFramework
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
            React19SSR.ConfigureReact19();
            Vue3SSR.ConfigureVue3();
        }
    }
}

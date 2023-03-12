using System.Web.Mvc;

namespace SSR.Net.React17DotNetFramework.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View();

        public ActionResult React17() => View();

        public ActionResult React18() => View();

        public ActionResult Vue3() => View();
    }
}
using System.Web.Mvc;

namespace SSR.Net.DotNetFramework.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View();

        public ActionResult React17() => View();

        public ActionResult React18() => View();

        public ActionResult React19() => View();

        public ActionResult Vue3() => View();
    }
}
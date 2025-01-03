using SSR.Net.React17DotNetFramework;
using SSR.Net.React18DotNetFramework;
using SSR.Net.React19DotNetFramework;
using SSR.Net.Vue3DotNetFramework;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SSR.Net.DotNetFramework.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View();

        public ActionResult React17()
        {
            var component = React17SSR.Render("Components.FrontPage", new {
                header = "React 17 with SSR",
                links = new[]{
                    new {
                        text = "Google.com",
                        href = "https://www.google.com"
                    },
                    new {
                        text = "Hacker news",
                        href = "https://news.ycombinator.org"
                    }
                }
            });
            return View(component);
        }

        public ActionResult React18()
        {
            var component = React18SSR.Render("Components.FrontPage", new {
                header = "React 18 with SSR",
                links = new[]{
                    new {
                        text = "Google.com",
                        href = "https://www.google.com"
                    },
                    new {
                        text = "Hacker news",
                        href ="https://news.ycombinator.org"
                    }
                }
            });
            return View(component);
        }

        public ActionResult React19()
        {
            var component = React19SSR.Render("Components.FrontPage", new {
                header = "React 19 with SSR",
                links = new[]{
                    new {
                        text= "Google.com",
                        href="https://www.google.com"
                    },
                    new {
                        text = "Hacker news",
                        href="https://news.ycombinator.org"
                    }
                }
            });
            return View(component);
        }

        public async Task<ActionResult> Vue3()
        {
            var component = await Vue3SSR.RenderAsync(
                "Components.Example", new {
                    title = "Vue 3 with SSR"
                });
            return View(component);
        }
    }
}
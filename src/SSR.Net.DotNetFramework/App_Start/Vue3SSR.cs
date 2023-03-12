using JavaScriptEngineSwitcher.V8;
using Newtonsoft.Json;
using SSR.Net.Models;
using SSR.Net.Services;
using System.Web;

namespace SSR.Net.React17DotNetFramework
{
    //Replace this with your preferred IoC-solution. 
    public class Vue3SSR
    {
        public static Vue3Renderer Renderer { get; private set; }
        public static void ConfigureVue3()
        {
            var pool = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                config
                    .AddScriptFile(HttpContext.Current.Server.MapPath("~/Frontend/vue3example.js"))
            );
            Renderer = new Vue3Renderer(pool);
        }

        public static RenderedComponent Render(string componentName, object props) =>
            Renderer.RenderComponent(componentName, JsonConvert.SerializeObject(props));
    }
}

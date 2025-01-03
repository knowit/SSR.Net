using JavaScriptEngineSwitcher.V8;
using Newtonsoft.Json;
using SSR.Net.Models;
using SSR.Net.Services;
using System.Threading.Tasks;
using System.Web;

namespace SSR.Net.Vue3DotNetFramework
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
                    .WithAsync()
            );
            Renderer = new Vue3Renderer(pool);
        }

        public static async Task<RenderedComponent> RenderAsync(string componentName, object props) =>
            await Renderer.RenderComponentAsync(componentName, JsonConvert.SerializeObject(props));
    }
}

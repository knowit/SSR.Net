using JavaScriptEngineSwitcher.V8;
using Newtonsoft.Json;
using SSR.Net.Models;
using SSR.Net.Services;
using System.Web;

namespace SSR.Net.React19DotNetFramework
{
    //Replace this with your preferred IoC-solution. 
    public class React19SSR
    {
        public static React19Renderer Renderer { get; private set; }
        public static void ConfigureReact19()
        {
            var pool = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                config
                    .AddScriptFile(HttpContext.Current.Server.MapPath("~/App_Start/React19TextEncoderPolyfill.js"))
                    .AddScriptFile(HttpContext.Current.Server.MapPath("~/App_Start/React19MessageChannelPolyfill.js"))
                    .AddScriptFile(HttpContext.Current.Server.MapPath("~/Frontend/react19example.js"))
            );
            Renderer = new React19Renderer(pool);
        }

        public static RenderedComponent Render(string componentName, object props) =>
            Renderer.RenderComponent(componentName, JsonConvert.SerializeObject(props));
    }
}

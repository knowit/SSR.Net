using JavaScriptEngineSwitcher.V8;
using Newtonsoft.Json;
using SSR.Net.Models;
using SSR.Net.Services;
using System.Web;

namespace SSR.Net.React17DotNetFramework
{
    //Replace this with your preferred IoC-solution. 
    public class React18SSR
    {
        public static React18Renderer Renderer { get; private set; }
        public static void ConfigureReact18()
        {
            var pool = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                config
                    .AddScriptFile(HttpContext.Current.Server.MapPath("~/App_Start/React18TextEncoderPolyfill.js"))
                    .AddScriptFile(HttpContext.Current.Server.MapPath("~/Frontend/react18example.js"))
            );
            Renderer = new React18Renderer(pool);
        }

        public static RenderedComponent Render(string componentName, object props) =>
            Renderer.RenderComponent(componentName, JsonConvert.SerializeObject(props));
    }
}

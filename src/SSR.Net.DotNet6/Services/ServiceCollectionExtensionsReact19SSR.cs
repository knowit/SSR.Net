using JavaScriptEngineSwitcher.V8;
using SSR.Net.Services;

namespace SSR.Net.DotNet6.Services
{
    public static class ServiceCollectionExtensionsReact19SSR
    {
        public static void AddReact19Renderer(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            var pool = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                config
                    .AddScriptFile(Path.Combine(webHostEnvironment.WebRootPath, "React19TextEncoderPolyfill.js"))
                    .AddScriptFile(Path.Combine(webHostEnvironment.WebRootPath, "react19example.js"))
            );
            services.AddSingleton(new React19Renderer(pool));
        }
    }
}

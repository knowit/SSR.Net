using JavaScriptEngineSwitcher.V8;
using SSR.Net.Services;

namespace SSR.Net.DotNet6.Services
{
    public static class ServiceCollectionExtensionsReact18SSR
    {
        public static void AddReact18Renderer(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            var pool = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                config
                    .AddScriptFile(Path.Combine(webHostEnvironment.WebRootPath, "React18TextEncoderPolyfill.js"))
                    .AddScriptFile(Path.Combine(webHostEnvironment.WebRootPath, "react18example.js"))
            );
            services.AddSingleton(new React18Renderer(pool));
        }
    }
}

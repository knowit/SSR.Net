using JavaScriptEngineSwitcher.V8;
using SSR.Net.Services;

namespace SSR.Net.DotNet6.Services
{
    public static class ServiceCollectionExtensionsReact17SSR
    {
        public static void AddReact17Renderer(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            var pool = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                config
                    .AddScriptFile(Path.Combine(webHostEnvironment.WebRootPath, "react17example.js"))
            );
            services.AddSingleton(new React17Renderer(pool));
        }
    }
}

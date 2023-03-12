using JavaScriptEngineSwitcher.V8;
using SSR.Net.Services;

namespace SSR.Net.DotNet6.Services
{
    public static class ServiceCollectionExtensionsVue3SSR
    {
        public static void AddVue3Renderer(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            var pool = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                config
                    .AddScriptFile(Path.Combine(webHostEnvironment.WebRootPath, "vue3example.js"))
            );
            services.AddSingleton(new Vue3Renderer(pool));
        }
    }
}

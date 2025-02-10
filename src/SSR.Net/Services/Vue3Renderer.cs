using SSR.Net.Exceptions;
using SSR.Net.Extensions;
using SSR.Net.Models;
using System;
using System.Threading.Tasks;

namespace SSR.Net.Services
{
    public class Vue3Renderer
    {
        private readonly IJavaScriptEnginePool _javaScriptEnginePool;

        public Vue3Renderer(IJavaScriptEnginePool javaScriptEnginePool) =>
            _javaScriptEnginePool = javaScriptEnginePool;

        private const string CSRHtml = "<div id=\"{0}\"></div>";//id
        private const string SSREngineScript =
            "renderToString(createSSRApp({0}, {1}))" +
            ".then(html => " + nameof(SSRNetResultCallback) + ".SetHtml('{2}','<div id={3}>' + html + '</div>'))" +
            ".catch(err => " + nameof(SSRNetResultCallback) + ".SetError('{2}','Error ' + err))";//componentName, propsAsJson, executionId, containerId

        private const string ClientHydrateScript = "createSSRApp({0}, {1}).mount({2})";//componentName, propsAsJson, id
        private const string ClientRenderScript = "createApp({0}, {1}).mount({2})";//id, componentName, propsAsJson

        public async Task<RenderedComponent> RenderComponentAsync(string componentName,
                                                 string propsAsJson,
                                                 int waitForEngineTimeoutMs = 50,
                                                 bool fallbackToClientSideRender = true,
                                                 int asyncTimeoutMs = 200,
                                                 bool sanitize = true)
        {
            var result = new RenderedComponent();
            var id = CreateExecutionId();
            var executionId = CreateExecutionId();
            string html = null;
            try {
                var js = string.Format(SSREngineScript, componentName, propsAsJson, executionId, id);
                html = await _javaScriptEnginePool.EvaluateJsAsync(js, executionId, asyncTimeoutMs, waitForEngineTimeoutMs);
            }
            catch (Exception ex) {
                if (!fallbackToClientSideRender)
                    throw ex;
                return FallbackToCSRWithException(componentName, propsAsJson, ex);
            }
            if (html is null)
                return RenderComponentCSR(componentName, propsAsJson);
            result.Html = html;
            result.InitScript = string.Format(ClientHydrateScript, componentName, propsAsJson, id);
            if (sanitize)
                result.InitScript = result.InitScript.SanitizeInitScript();
            return result;
        }

        private RenderedComponent FallbackToCSRWithException(string componentName, string propsAsJson, Exception ex)
        {
            var result = RenderComponentCSR(componentName, propsAsJson);
            if (ex is AcquireJavaScriptEngineTimeoutException timeoutException)
                result.TimeoutException = timeoutException;
            else
                result.RenderException = ex;
            return result;
        }

        private static string CreateExecutionId() =>
            "vue_" + Guid.NewGuid().ToString().Replace("-", "");

        public RenderedComponent RenderComponentCSR(string componentName, string propsAsJson, bool sanitize = true)
        {
            var id = CreateExecutionId();
            var result = new RenderedComponent
            {
                Html = string.Format(CSRHtml, id),
                InitScript = string.Format(ClientRenderScript, componentName, propsAsJson, id)
            };
            if (sanitize)
                result.InitScript = result.InitScript.SanitizeInitScript();
            return result;
        }
    }
}

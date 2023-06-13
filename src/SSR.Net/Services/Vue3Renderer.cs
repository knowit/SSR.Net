using SSR.Net.Exceptions;
using SSR.Net.Models;
using System;

namespace SSR.Net.Services
{
    public class Vue3Renderer
    {
        private readonly IJavaScriptEnginePool _javaScriptEnginePool;

        public Vue3Renderer(IJavaScriptEnginePool javaScriptEnginePool) =>
            _javaScriptEnginePool = javaScriptEnginePool;

        private const string CSRHtml = "<div id=\"{0}\"></div>";//id
        private const string SSREngineScript = "renderToString(createSSRApp({0}, {1})).then(html => {2}= '<div id={3}>' + html + '</div>').catch(err => {2}= 'Error ' + err)";//componentName, propsAsJson, resultVariableName, containerId
        private const string ClientHydrateScript = "createSSRApp({0}, {1}).mount({2})";//componentName, propsAsJson, id
        private const string ClientRenderScript = "createApp({0}, {1}).mount({2})";//id, componentName, propsAsJson

        public RenderedComponent RenderComponent(string componentName,
                                                 string propsAsJson,
                                                 int waitForEngineTimeoutMs = 50,
                                                 bool fallbackToClientSideRender = true,
                                                 int asyncTimeoutMs = 200)
        {
            var result = new RenderedComponent();
            var id = CreateId();
            var variableId = CreateId();
            string html = null;
            try
            {
                html = _javaScriptEnginePool.EvaluateJsAsync(string.Format(SSREngineScript, componentName, propsAsJson, variableId, id), variableId, asyncTimeoutMs, waitForEngineTimeoutMs);
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
            return result;
        }

        private RenderedComponent FallbackToCSRWithException(string componentName, string propsAsJson, Exception ex) {
            var result = RenderComponentCSR(componentName, propsAsJson);
            if (ex is AcquireJavaScriptEngineTimeoutException timeoutException)
                result.TimeoutException = timeoutException;
            else
                result.RenderException = ex;
            return result;
        }

        private static string CreateId() =>
            "vue_" + Guid.NewGuid().ToString().Replace("-", "");

        public RenderedComponent RenderComponentCSR(string componentName, string propsAsJson)
        {
            var id = CreateId();
            return new RenderedComponent
            {
                Html = string.Format(CSRHtml, id),
                InitScript = string.Format(ClientRenderScript, componentName, propsAsJson, id)
            };
        }
    }
}

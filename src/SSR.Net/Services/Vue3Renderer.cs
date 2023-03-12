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
        private const string ClientRenderScript = "ReactDOMClient.createRoot({0}).render(React.createElement({1},{2}))";//id, componentName, propsAsJson

        public RenderedComponent RenderComponent(string componentName,
                                                 string propsAsJson,
                                                 int waitForEngineTimeoutMs = 50,
                                                 bool fallbackToClientSideRender = true)
        {
            var result = new RenderedComponent();
            var id = CreateId();
            var variableId = CreateId();
            result.Html = _javaScriptEnginePool.EvaluateJsAsync(string.Format(SSREngineScript, componentName, propsAsJson, variableId, id), variableId);
            result.InitScript = string.Format(ClientHydrateScript, componentName, propsAsJson, id);
            //var script = string.Format(SSREngineScript, componentName, propsAsJson);
            //result.Html = string.Format(SSRHtml, id, _javaScriptEnginePool.EvaluateJs(script, waitForEngineTimeoutMs, fallbackToClientSideRender));
            //result.InitScript = string.Format(ClientHydrateScript, id, componentName, propsAsJson);
            //if (result.Html is null)
            //    return RenderComponentCSR(componentName, propsAsJson);
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
                InitScript = string.Format(ClientRenderScript, id, componentName, propsAsJson)
            };
        }
    }
}

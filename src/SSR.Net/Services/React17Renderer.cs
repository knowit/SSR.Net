﻿using SSR.Net.Exceptions;
using SSR.Net.Extensions;
using SSR.Net.Models;
using System;

namespace SSR.Net.Services
{
    public class React17Renderer
    {
        private readonly IJavaScriptEnginePool _javaScriptEnginePool;

        public React17Renderer(IJavaScriptEnginePool javaScriptEnginePool) =>
            _javaScriptEnginePool = javaScriptEnginePool;

        private const string SSRHtml = "<div id=\"{0}\">{1}</div>";//id, html
        private const string CSRHtml = "<div id=\"{0}\"></div>";//id
        private const string SSREngineScript = "ReactDOMServer.renderToString(React.createElement({0},{1}))";//componentName, propsAsJson
        private const string ClientHydrateScript = "ReactDOM.hydrate(React.createElement({0},{1}), {2})";//componentName, propsAsJson, id
        private const string ClientRenderScript = "ReactDOM.render(React.createElement({0},{1}), {2})";//componentName, propsAsJson, id

        public RenderedComponent RenderComponent(string componentName,
                                                 string propsAsJson,
                                                 int waitForEngineTimeoutMs = 50,
                                                 bool fallbackToClientSideRender = true,
                                                 bool sanitize = true)
        {
            var result = new RenderedComponent();
            var id = CreateId();
            var script = string.Format(SSREngineScript, componentName, propsAsJson);
            string html;
            try {
                html = _javaScriptEnginePool.EvaluateJs(script, waitForEngineTimeoutMs);
            }
            catch (Exception ex) {
                if (!fallbackToClientSideRender)
                    throw ex;
                return FallbackToCSRWithException(componentName, propsAsJson, ex, sanitize);
            }
            if (html is null)
                return RenderComponentCSR(componentName, propsAsJson, sanitize);
            result.Html = string.Format(SSRHtml, id, html);
            result.InitScript = string.Format(ClientHydrateScript, componentName, propsAsJson, id);
            if (sanitize)
                result.InitScript = result.InitScript.SanitizeInitScript();
            return result;
        }

        private RenderedComponent FallbackToCSRWithException(string componentName, string propsAsJson, Exception ex, bool sanitize)
        {
            var result = RenderComponentCSR(componentName, propsAsJson, sanitize);
            if (ex is AcquireJavaScriptEngineTimeoutException timeoutException)
                result.TimeoutException = timeoutException;
            else
                result.RenderException = ex;
            return result;
        }

        private static string CreateId() =>
            "react_" + Guid.NewGuid().ToString().Replace("-", "");

        public RenderedComponent RenderComponentCSR(string componentName, string propsAsJson, bool sanitize = true)
        {
            var id = CreateId();
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

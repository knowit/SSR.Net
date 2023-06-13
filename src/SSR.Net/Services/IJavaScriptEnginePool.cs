using SSR.Net.Models;
using System;

namespace SSR.Net.Services
{
    public interface IJavaScriptEnginePool
    {
        string EvaluateJs(string js, int timeoutMs = 200);
        string EvaluateJsAsync(string js, string resultVariableName, int asyncTimeoutMs = 200, int timeoutMs = 200);
        JavaScriptEnginePoolStats GetStats();
        JavaScriptEnginePool Reconfigure(Func<JavaScriptEnginePoolConfig, JavaScriptEnginePoolConfig> config);
    }
}

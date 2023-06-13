using SSR.Net.Exceptions;
using System;

namespace SSR.Net.Models
{
    public class RenderedComponent
    {
        public string Html { get; set; }
        public string InitScript { get; set; }
        public Exception RenderException { get; set; }
        public AcquireJavaScriptEngineTimeoutException TimeoutException { get; set; }
    }
}

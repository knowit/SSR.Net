using System;

namespace SSR.Net.Models
{
    public class JavaScriptEngineStats
    {
        public JavaScriptEngineState State { get; set; }
        public DateTime InstantiatedTime { get; set; }
        public DateTime InitializedTime { get; set; }
        public int UsageCount { get; set; }
        public int BundleNumber { get; set; }
        public Exception InitializationException { get; internal set; }
    }
}

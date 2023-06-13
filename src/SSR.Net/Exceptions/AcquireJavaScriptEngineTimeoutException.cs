using System;

namespace SSR.Net.Exceptions {
    public class AcquireJavaScriptEngineTimeoutException : Exception {
        public AcquireJavaScriptEngineTimeoutException(string message) : base(message) { }
    }
}

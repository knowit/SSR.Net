using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SSR.Net.Models
{
    public enum ResultCallbackState
    {
        AwaitingCode,
        AwaitingResult,
        ResultAvailable
    }

    public class SSRNetResultCallback
    {
        private string _html { get; set; }
        private string _error { get; set; }
        private ResultCallbackState _state { get; set; } = ResultCallbackState.AwaitingCode;
        private string _executionId { get; set; }
        private object _lockObject { get; } = new object();

        public void SetExecutionId(string executionId)
        {
            lock (_lockObject) {
                _html = null;
                _error = null;
                _executionId = executionId;
                _state = ResultCallbackState.AwaitingResult;
            }
        }

        public void SetHtml(string executionId, string html) =>
            SetResult(executionId, html, null);

        public void SetError(string executionId, string error) =>
            SetResult(executionId, null, error);

        private void SetResult(string executionId, string html, string error)
        {
            lock (_lockObject) {
                if (_state == ResultCallbackState.AwaitingResult && _executionId == executionId) {
                    _html = html;
                    _error = error;
                    _state = ResultCallbackState.ResultAvailable;
                }
            }
        }

        public async Task AwaitResult(int timeoutMs)
        {
            var sw = Stopwatch.StartNew();
            while (_state != ResultCallbackState.ResultAvailable && sw.ElapsedMilliseconds < timeoutMs)
                await Task.Delay(1);
            lock (_lockObject) {
                if (_state != ResultCallbackState.ResultAvailable) {
                    _state = ResultCallbackState.ResultAvailable;
                    _error = $"Timeout after {timeoutMs}ms";
                }
            }
        }

        public bool HasHtml() => !(_html is null);

        public bool HasError() => !(_error is null);

        internal string RetrieveResult(string executionId)
        {
            lock (_lockObject) {
                if (_state != ResultCallbackState.ResultAvailable)
                    throw new InvalidOperationException("Result not available.");
                if (_executionId != executionId)
                    throw new InvalidOperationException("Execution ID didn't match.");
                var result = _html;
                _html = null;
                _error = null;
                _executionId = null;
                _state = ResultCallbackState.AwaitingCode;
                return result;
            }
        }
    }
}

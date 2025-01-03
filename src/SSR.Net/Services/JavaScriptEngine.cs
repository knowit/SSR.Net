using JavaScriptEngineSwitcher.Core;
using SSR.Net.Models;
using System;
using System.Threading.Tasks;

namespace SSR.Net.Services
{
    public class JavaScriptEngine : IDisposable
    {
        protected IJsEngine _engine;
        protected JavaScriptEngineState _state;
        protected readonly int _maxUsages;
        protected readonly int _garbageCollectionInterval;
        public int BundleNumber { get; }
        public int UsageCount { get; protected set; }
        protected bool _depleted;
        protected Task _initializer;
        public DateTime InstantiationTime { get; protected set; }
        public DateTime InitializedTime { get; protected set; }
        public Exception InitializationException { get; protected set; }
        protected SSRNetResultCallback ResultCallback { get; set; }

        public JavaScriptEngine(Func<IJsEngine> createEngine, int maxUsages, int garbageCollectionInterval, int bundleNumber, bool supportAsync)
        {
            _maxUsages = maxUsages;
            BundleNumber = bundleNumber;
            InstantiationTime = DateTime.UtcNow;
            _garbageCollectionInterval = garbageCollectionInterval;
            _state = JavaScriptEngineState.Uninitialized;
            _initializer = Task.Run(() => {
                try {
                    _engine = createEngine();
                    if (supportAsync) {
                        ResultCallback = new SSRNetResultCallback();
                        _engine.EmbedHostObject(nameof(SSRNetResultCallback), ResultCallback);
                    }
                    _state = JavaScriptEngineState.Ready;
                    InitializedTime = DateTime.UtcNow;
                }
                catch (Exception ex) {
                    _state = JavaScriptEngineState.InitializationFailed;
                    InitializationException = ex;
                    InitializedTime = DateTime.UtcNow;
                    throw;
                }
            });
        }

        public virtual JavaScriptEngineState GetState() => _depleted ? JavaScriptEngineState.Depleted : _state;
        public virtual bool IsLeased => GetState() == JavaScriptEngineState.Leased;
        public virtual bool IsReady => GetState() == JavaScriptEngineState.Ready;
        public virtual bool IsDepleted => GetState() == JavaScriptEngineState.Depleted;

        public virtual JavaScriptEngine Lease()
        {
            if (!IsReady)
                throw new InvalidOperationException($"Cannot lease engine when the engine is in state {_state}");
            _state = JavaScriptEngineState.Leased;
            UsageCount++;
            return this;
        }

        public virtual async Task<string> EvaluateAsyncAndRelease(string script, string executionId, int asyncTimeoutMs)
        {
            if (_state != JavaScriptEngineState.Leased)
                throw new InvalidOperationException($"Cannot evaluate script on engine in state {_state}");
            try {
                ResultCallback.SetExecutionId(executionId);
                _engine.Execute(script);
                await ResultCallback.AwaitResult(asyncTimeoutMs);
                if (ResultCallback.HasHtml())
                    return ResultCallback.RetrieveResult(executionId);
            }
            finally {
                Release();
            }
            return null;
        }

        public virtual string EvaluateAndRelease(string script)
        {
            //This engine instance might be depleted if the pool was restarted, but the engine should finish
            //its render to avoid 500 errors on the web
            if (_state != JavaScriptEngineState.Leased)
                throw new InvalidOperationException($"Cannot evaluate script on engine in state {_state}");
            string result;
            try {
                result = _engine.Evaluate<string>(script);
            }
            finally {
                Release();
            }
            return result;
        }

        protected virtual void Release()
        {
            if (UsageCount >= _maxUsages)
                _depleted = true;
            else if (UsageCount % _garbageCollectionInterval == 0) {
                _state = JavaScriptEngineState.RequiresGarbageCollection;
                Task.Run(() => {
                    RunGarbageCollection();
                    _state = JavaScriptEngineState.Ready;
                });
            }
            else {
                _state = JavaScriptEngineState.Ready;
            }
        }

        public virtual void Dispose()
        {
            _initializer.Wait();
            _engine.Dispose();
        }

        public virtual void SetDepleted() =>
            _depleted = true;

        public virtual void RunGarbageCollection()
        {
            if (_engine.SupportsGarbageCollection) {
                _engine.CollectGarbage();
                _state = JavaScriptEngineState.Ready;
            }
        }
    }
}

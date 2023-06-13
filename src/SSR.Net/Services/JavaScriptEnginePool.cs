using JavaScriptEngineSwitcher.Core;
using SSR.Net.Exceptions;
using SSR.Net.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SSR.Net.Services
{
    public class JavaScriptEnginePool : IJavaScriptEnginePool
    {
        protected List<string> Scripts;
        protected List<JavaScriptEngine> Engines = new List<JavaScriptEngine>();
        protected JsEngineSwitcher JsEngineSwitcher;
        protected int GarbageCollectionInterval = 20;
        protected int ReconfigureTimeoutMs = 2000;
        protected int BundleNumber = 0;
        protected int MaxEngines = 25;
        protected int MaxUsages = 100;
        protected int MinEngines = 5;
        protected int StandbyEngineCount = 3;
        protected object Lock = new object();
        public bool IsStarted { get; protected set; }

        public JavaScriptEnginePool(IJsEngineFactory jsEngineFactory, Func<JavaScriptEnginePoolConfig, JavaScriptEnginePoolConfig> config)
        {
            JsEngineSwitcher = new JsEngineSwitcher();
            JsEngineSwitcher.EngineFactories.Add(jsEngineFactory);
            JsEngineSwitcher.DefaultEngineName = jsEngineFactory.EngineName;
            Reconfigure(config);
        }

        public virtual string EvaluateJs(string js, int timeoutMs = 200) {
            var engine = GetEngine(timeoutMs);
            if (engine is null)
                throw new AcquireJavaScriptEngineTimeoutException($"Could not acquire engine within {timeoutMs}ms");
            return engine.EvaluateAndRelease(js);
        }

        public virtual string EvaluateJsAsync(string js, string resultVariableName, int asyncTimeoutMs = 200, int timeoutMs = 200)
        {
            var engine = GetEngine(timeoutMs);
            if (engine is null)
                throw new AcquireJavaScriptEngineTimeoutException($"Could not acquire engine within {timeoutMs}ms");
            var result = engine.EvaluateAsyncAndRelease(js, resultVariableName, asyncTimeoutMs);
            if (!string.IsNullOrEmpty(result))
                return result;
            throw new Exception($"Could not evaluate async result within {asyncTimeoutMs}ms");
        }

        protected virtual JavaScriptEngine GetEngine(int timeoutMs)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                lock (Lock)
                {
                    RemoveDepletedEngines();
                    ThrowExceptionIfInitializationFailed();
                    RefillToMinEngines();
                    EnsureEnoughStandbyEngines();
                    var engine = TryToFindReadyEngine();
                    if (engine != null)
                    {
                        var result = engine.Lease();
                        EnsureEnoughStandbyEngines();//We ensure that there are enough standby engines after the lease
                        return result;
                    }
                }
                Thread.Sleep(5);
            }
            Console.WriteLine($"Leasing took {sw.ElapsedMilliseconds}ms, but no engine was found");
            return null;
        }

        protected virtual void EnsureEnoughStandbyEngines()
        {
            var neededStandbyEngines = StandbyEngineCount - Engines.Count(e => !e.IsDepleted && !e.IsLeased);
            if (neededStandbyEngines <= 0)
                return;
            var maxNewStandbyEngines = MaxEngines - Engines.Count();
            var toInstantiate = Math.Min(neededStandbyEngines, maxNewStandbyEngines);
            for (int i = 0; i < toInstantiate; ++i)
                AddNewJsEngine();
        }

        protected virtual JavaScriptEngine TryToFindReadyEngine() =>
            GetEnginesSortedByUsageThenAge()
                .FirstOrDefault(e => e.IsReady);

        protected virtual JavaScriptEngine[] GetEnginesSortedByUsageThenAge() =>
            Engines
                .OrderByDescending(e => e.UsageCount)
                .ThenBy(e => e.InstantiationTime)
                .ToArray();

        protected virtual void RefillToMinEngines()
        {
            while (Engines.Count < MinEngines)
                AddNewJsEngine();
        }

        protected virtual void RemoveDepletedEngines() =>
            Engines
                .Where(e => e.IsDepleted)
                .ToList()
                .ForEach(e => { Engines.Remove(e); e.Dispose(); });

        public virtual JavaScriptEnginePool Reconfigure(Func<JavaScriptEnginePoolConfig, JavaScriptEnginePoolConfig> config)
        {
            var builtConfig = config(new JavaScriptEnginePoolConfig());
            builtConfig.Validate();
            lock (Lock)
            {
                Engines.Where(e => !e.IsLeased).ToList().ForEach(e => { Engines.Remove(e); e.Dispose(); });
                Engines.ForEach(e => e.SetDepleted());
                MaxUsages = builtConfig.MaxUsages;
                MaxEngines = builtConfig.MaxEngines;
                MinEngines = builtConfig.MinEngines;
                Scripts = builtConfig.Scripts;
                StandbyEngineCount = builtConfig.StandbyEngineCount;
                GarbageCollectionInterval = builtConfig.GarbageCollectionInterval;
                ReconfigureTimeoutMs = builtConfig.ReconfigureTimeoutMs;
                BundleNumber++;
                for (int i = 0; i < MinEngines; ++i)
                    AddNewJsEngine();
                var startWaitForInit = DateTime.Now;
                while (IsInitializingAllEngines() && WaitingForInitializationNotTimedOut(startWaitForInit))
                    Thread.Sleep(10);
                ThrowExceptionIfInitializationFailed();
                IsStarted = true;
            }
            return this;
        }

        private bool WaitingForInitializationNotTimedOut(DateTime startWaitForInit) =>
            (DateTime.Now - startWaitForInit).TotalMilliseconds < ReconfigureTimeoutMs;

        private bool IsInitializingAllEngines() =>
            !Engines.Any(e => new[] { JavaScriptEngineState.Ready, JavaScriptEngineState.InitializationFailed }.Contains(e.GetState()));

        private void ThrowExceptionIfInitializationFailed()
        {
            if (Engines.FirstOrDefault(e => e.GetState().Equals(JavaScriptEngineState.InitializationFailed)) is JavaScriptEngine jsEngine
                && jsEngine.InitializationException != null)
                throw jsEngine.InitializationException;
        }

        protected virtual void AddNewJsEngine() =>
            Engines.Add(new JavaScriptEngine(() =>
            {
                var jsEngine = JsEngineSwitcher.CreateDefaultEngine();
                Scripts.ForEach(jsEngine.Execute);
                return jsEngine;
            }, MaxUsages, GarbageCollectionInterval, BundleNumber));

        public virtual JavaScriptEnginePoolStats GetStats() =>
            new JavaScriptEnginePoolStats
            {
                EngineCount = Engines.Count,
                EngineStats = Engines
                    .Select(engine =>
                        new JavaScriptEngineStats
                        {
                            BundleNumber = engine.BundleNumber,
                            InitializedTime = engine.InitializedTime,
                            InstantiatedTime = engine.InstantiationTime,
                            State = engine.GetState(),
                            InitializationException = engine.InitializationException,
                            UsageCount = engine.UsageCount
                        })
                    .ToList()
            };
    }
}

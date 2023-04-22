namespace SSR.Net.Models
{
    public enum JavaScriptEngineState
    {
        Uninitialized,
        InitializationFailed,
        Ready,
        Leased,
        RequiresGarbageCollection,
        Depleted
    }
}

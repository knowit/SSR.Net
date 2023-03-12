namespace SSR.Net.Models
{
    public enum JavaScriptEngineState
    {
        Uninitialized,
        Ready,
        Leased,
        RequiresGarbageCollection,
        Depleted
    }
}

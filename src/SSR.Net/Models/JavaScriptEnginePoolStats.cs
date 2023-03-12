using System.Collections.Generic;

namespace SSR.Net.Models
{
    public class JavaScriptEnginePoolStats
    {
        public int EngineCount { get; set; }
        public List<JavaScriptEngineStats> EngineStats { get; set; } = new List<JavaScriptEngineStats>();
    }
}

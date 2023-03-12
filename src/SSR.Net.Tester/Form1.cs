using JavaScriptEngineSwitcher.V8;
using SSR.Net.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSR.Net.Tester
{
    public partial class Form1 : Form
    {
        private JavaScriptEnginePool _jsep;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartPool_Click(object sender, EventArgs e)
        {
            _jsep = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                            config
                                .WithMaxUsagesCount(50)
                                .WithMaxEngineCount(15)
                                .AddScript("const Test = (i)=> \"Hello from function. i+i=\" + (i+i);")
                    );
        }

        private void ExecuteJs_Click(object sender, EventArgs e)
        {
            try
            {
                var res = _jsep.EvaluateJs(JS.Text);
                Result.Text = res;
            }
            catch (Exception ex)
            {
                Result.Text = ex.Message;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_jsep != null && _jsep.IsStarted)
            {
                    var stats = _jsep.GetStats();
                var sb = new StringBuilder();
                sb.AppendLine($"Engines ({stats.EngineCount}):");
                foreach (var engineStats in stats.EngineStats)
                sb.AppendLine($"Usages: {engineStats.UsageCount}, Instantiated {engineStats.InstantiatedTime}, Initialization time {engineStats.InitializedTime - engineStats.InstantiatedTime}, Bundle number {engineStats.BundleNumber}, State {engineStats.State}");
                Stats.Text = sb.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task t = new Task(() =>
            {
                try
                {
                    var res = _jsep.EvaluateJs(JS.Text, returnNullInsteadOfException: true);

                }
                catch (Exception ex)
                { }
            });
            t.Start();
        }

        private void RestartPool_Click(object sender, EventArgs e)
        {
            _jsep.Reconfigure(config =>
                config
                .AddScript("const Test = (i)=> \"Hello from function. i+i=\" + (i+i);")
                .WithMaxUsagesCount(50)
            );

        }
    }
}

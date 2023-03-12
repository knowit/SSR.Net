namespace SSR.Net.Tester
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.StartPool = new System.Windows.Forms.Button();
            this.ExecuteJs = new System.Windows.Forms.Button();
            this.JS = new System.Windows.Forms.TextBox();
            this.Result = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.Stats = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.RestartPool = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartPool
            // 
            this.StartPool.Location = new System.Drawing.Point(12, 12);
            this.StartPool.Name = "StartPool";
            this.StartPool.Size = new System.Drawing.Size(188, 23);
            this.StartPool.TabIndex = 0;
            this.StartPool.Text = "Start pool";
            this.StartPool.UseVisualStyleBackColor = true;
            this.StartPool.Click += new System.EventHandler(this.StartPool_Click);
            // 
            // ExecuteJs
            // 
            this.ExecuteJs.Location = new System.Drawing.Point(12, 68);
            this.ExecuteJs.Name = "ExecuteJs";
            this.ExecuteJs.Size = new System.Drawing.Size(188, 23);
            this.ExecuteJs.TabIndex = 1;
            this.ExecuteJs.Text = "Execute JS";
            this.ExecuteJs.UseVisualStyleBackColor = true;
            this.ExecuteJs.Click += new System.EventHandler(this.ExecuteJs_Click);
            // 
            // JS
            // 
            this.JS.Location = new System.Drawing.Point(231, 14);
            this.JS.Multiline = true;
            this.JS.Name = "JS";
            this.JS.Size = new System.Drawing.Size(426, 232);
            this.JS.TabIndex = 2;
            this.JS.Text = "Test(5)";
            // 
            // Result
            // 
            this.Result.Location = new System.Drawing.Point(231, 252);
            this.Result.Name = "Result";
            this.Result.Size = new System.Drawing.Size(426, 20);
            this.Result.TabIndex = 3;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Stats
            // 
            this.Stats.Location = new System.Drawing.Point(663, 14);
            this.Stats.Multiline = true;
            this.Stats.Name = "Stats";
            this.Stats.Size = new System.Drawing.Size(551, 548);
            this.Stats.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 97);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(188, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Execute on background thread";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RestartPool
            // 
            this.RestartPool.Location = new System.Drawing.Point(12, 39);
            this.RestartPool.Name = "RestartPool";
            this.RestartPool.Size = new System.Drawing.Size(188, 23);
            this.RestartPool.TabIndex = 6;
            this.RestartPool.Text = "Restart pool";
            this.RestartPool.UseVisualStyleBackColor = true;
            this.RestartPool.Click += new System.EventHandler(this.RestartPool_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1263, 667);
            this.Controls.Add(this.RestartPool);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Stats);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.JS);
            this.Controls.Add(this.ExecuteJs);
            this.Controls.Add(this.StartPool);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartPool;
        private System.Windows.Forms.Button ExecuteJs;
        private System.Windows.Forms.TextBox JS;
        private System.Windows.Forms.TextBox Result;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox Stats;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button RestartPool;
    }
}


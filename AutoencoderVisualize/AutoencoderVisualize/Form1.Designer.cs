namespace AutoencoderVisualize
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SubDimGraph = new AutoencoderVisualize.DimGraph();
            this.colorBar = new AutoencoderVisualize.ColorBar();
            this.dimGraph = new AutoencoderVisualize.DimGraph();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(147, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SubDimGraph
            // 
            this.SubDimGraph.BackColor = System.Drawing.Color.Transparent;
            this.SubDimGraph.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SubDimGraph.ForeColor = System.Drawing.Color.Black;
            this.SubDimGraph.Location = new System.Drawing.Point(403, 99);
            this.SubDimGraph.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SubDimGraph.Name = "SubDimGraph";
            this.SubDimGraph.Size = new System.Drawing.Size(612, 565);
            this.SubDimGraph.TabIndex = 3;
            // 
            // colorBar
            // 
            this.colorBar.BackColor = System.Drawing.Color.Transparent;
            this.colorBar.Location = new System.Drawing.Point(888, 103);
            this.colorBar.Name = "colorBar";
            this.colorBar.Size = new System.Drawing.Size(148, 323);
            this.colorBar.TabIndex = 2;
            // 
            // dimGraph
            // 
            this.dimGraph.BackColor = System.Drawing.Color.Transparent;
            this.dimGraph.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dimGraph.ForeColor = System.Drawing.Color.Black;
            this.dimGraph.Location = new System.Drawing.Point(165, 64);
            this.dimGraph.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dimGraph.Name = "dimGraph";
            this.dimGraph.Size = new System.Drawing.Size(612, 565);
            this.dimGraph.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 653);
            this.Controls.Add(this.SubDimGraph);
            this.Controls.Add(this.colorBar);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dimGraph);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DimGraph dimGraph;
        private System.Windows.Forms.Button button1;
        private ColorBar colorBar;
        private DimGraph SubDimGraph;

    }
}


namespace AutoencoderVisualize
{
    partial class ColorBar
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.bar = new System.Windows.Forms.PictureBox();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.bar)).BeginInit();
            this.SuspendLayout();
            // 
            // bar
            // 
            this.bar.BackColor = System.Drawing.Color.Transparent;
            this.bar.Location = new System.Drawing.Point(0, 136);
            this.bar.Name = "bar";
            this.bar.Size = new System.Drawing.Size(100, 50);
            this.bar.TabIndex = 0;
            this.bar.TabStop = false;
            this.bar.Click += new System.EventHandler(this.bar_Click);
            this.bar.Paint += new System.Windows.Forms.PaintEventHandler(this.bar_Paint);
            // 
            // ColorBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.Controls.Add(this.bar);
            this.Name = "ColorBar";
            this.Size = new System.Drawing.Size(148, 323);
            this.Load += new System.EventHandler(this.ColorBar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox bar;
        private System.Windows.Forms.ColorDialog colorPicker;
    }
}

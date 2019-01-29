namespace AutoencoderVisualize
{
    partial class DimGraph
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
            this.grid = new System.Windows.Forms.PictureBox();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.BackColor = System.Drawing.Color.Transparent;
            this.grid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(266, 375);
            this.grid.TabIndex = 0;
            this.grid.TabStop = false;
            this.grid.Paint += new System.Windows.Forms.PaintEventHandler(this.grid_Paint);
            // 
            // DimGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SeaShell;
            this.Controls.Add(this.grid);
            this.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DimGraph";
            this.Size = new System.Drawing.Size(954, 652);
            this.Load += new System.EventHandler(this.DimGraph_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox grid;
        private System.Windows.Forms.ColorDialog colorPicker;
    }
}

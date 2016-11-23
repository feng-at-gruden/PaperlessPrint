namespace SignBoard
{
    partial class PDFViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFViewer));
            this.foxitReader1 = new AxFoxitReaderSDKProLib.AxFoxitReaderSDK();
            ((System.ComponentModel.ISupportInitialize)(this.foxitReader1)).BeginInit();
            this.SuspendLayout();
            // 
            // foxitReader1
            // 
            this.foxitReader1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.foxitReader1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foxitReader1.Enabled = true;
            this.foxitReader1.Location = new System.Drawing.Point(0, 0);
            this.foxitReader1.Margin = new System.Windows.Forms.Padding(0);
            this.foxitReader1.Name = "foxitReader1";
            this.foxitReader1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("foxitReader1.OcxState")));
            this.foxitReader1.Size = new System.Drawing.Size(150, 150);
            this.foxitReader1.TabIndex = 0;
            // 
            // PDFViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.foxitReader1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PDFViewer";
            ((System.ComponentModel.ISupportInitialize)(this.foxitReader1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxFoxitReaderSDKProLib.AxFoxitReaderSDK foxitReader1;
    }
}

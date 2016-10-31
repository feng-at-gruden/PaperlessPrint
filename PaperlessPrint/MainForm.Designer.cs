namespace PaperlessPrint
{
    partial class MainForm
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
            this.btnTabletShow = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnConfirmSign = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTabletShow
            // 
            this.btnTabletShow.Location = new System.Drawing.Point(73, 34);
            this.btnTabletShow.Name = "btnTabletShow";
            this.btnTabletShow.Size = new System.Drawing.Size(84, 39);
            this.btnTabletShow.TabIndex = 0;
            this.btnTabletShow.Text = "客户确认";
            this.btnTabletShow.UseVisualStyleBackColor = true;
            this.btnTabletShow.Click += new System.EventHandler(this.btnTabletShow_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(73, 188);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(84, 39);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "物理打印";
            this.btnPrint.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(73, 265);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 39);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "完成";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnConfirmSign
            // 
            this.btnConfirmSign.Location = new System.Drawing.Point(73, 111);
            this.btnConfirmSign.Name = "btnConfirmSign";
            this.btnConfirmSign.Size = new System.Drawing.Size(84, 39);
            this.btnConfirmSign.TabIndex = 1;
            this.btnConfirmSign.Text = "签字确认";
            this.btnConfirmSign.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 335);
            this.Controls.Add(this.btnConfirmSign);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnTabletShow);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "无纸化签名";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDoubleClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTabletShow;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnConfirmSign;
    }
}


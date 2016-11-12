namespace PaperlessPrint
{
    partial class SettingForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtTabletAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTempFileFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFtpAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFtpUserName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFtpPassword = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTabletPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "签名设备地址:";
            // 
            // txtTabletAddress
            // 
            this.txtTabletAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTabletAddress.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTabletAddress.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTabletAddress.Location = new System.Drawing.Point(128, 20);
            this.txtTabletAddress.MaxLength = 15;
            this.txtTabletAddress.Name = "txtTabletAddress";
            this.txtTabletAddress.Size = new System.Drawing.Size(114, 21);
            this.txtTabletAddress.TabIndex = 0;
            this.txtTabletAddress.Text = "192.168.1.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "临时存储路径:";
            // 
            // txtTempFileFolder
            // 
            this.txtTempFileFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTempFileFolder.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTempFileFolder.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTempFileFolder.Location = new System.Drawing.Point(128, 65);
            this.txtTempFileFolder.MaxLength = 15;
            this.txtTempFileFolder.Name = "txtTempFileFolder";
            this.txtTempFileFolder.Size = new System.Drawing.Size(177, 21);
            this.txtTempFileFolder.TabIndex = 2;
            this.txtTempFileFolder.Text = "\\\\192.168.1.1\\Paperless\\tmp";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "文件服务器地址:";
            // 
            // txtFtpAddress
            // 
            this.txtFtpAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFtpAddress.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFtpAddress.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtFtpAddress.Location = new System.Drawing.Point(128, 136);
            this.txtFtpAddress.MaxLength = 15;
            this.txtFtpAddress.Name = "txtFtpAddress";
            this.txtFtpAddress.Size = new System.Drawing.Size(114, 21);
            this.txtFtpAddress.TabIndex = 3;
            this.txtFtpAddress.Text = "192.168.1.202";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(269, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "FTP帐号:";
            // 
            // txtFtpUserName
            // 
            this.txtFtpUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFtpUserName.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFtpUserName.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtFtpUserName.Location = new System.Drawing.Point(328, 136);
            this.txtFtpUserName.MaxLength = 15;
            this.txtFtpUserName.Name = "txtFtpUserName";
            this.txtFtpUserName.Size = new System.Drawing.Size(83, 21);
            this.txtFtpUserName.TabIndex = 4;
            this.txtFtpUserName.Text = "192.168.1.202";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(269, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "FTP密码:";
            // 
            // txtFtpPassword
            // 
            this.txtFtpPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFtpPassword.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFtpPassword.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtFtpPassword.Location = new System.Drawing.Point(328, 167);
            this.txtFtpPassword.MaxLength = 15;
            this.txtFtpPassword.Name = "txtFtpPassword";
            this.txtFtpPassword.PasswordChar = '*';
            this.txtFtpPassword.Size = new System.Drawing.Size(83, 21);
            this.txtFtpPassword.TabIndex = 5;
            this.txtFtpPassword.Text = "123456";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(178, 207);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(287, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "端口:";
            // 
            // txtTabletPort
            // 
            this.txtTabletPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTabletPort.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTabletPort.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTabletPort.Location = new System.Drawing.Point(328, 20);
            this.txtTabletPort.MaxLength = 15;
            this.txtTabletPort.Name = "txtTabletPort";
            this.txtTabletPort.Size = new System.Drawing.Size(83, 21);
            this.txtTabletPort.TabIndex = 1;
            this.txtTabletPort.Text = "7654";
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 251);
            this.Controls.Add(this.txtTabletPort);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtFtpPassword);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFtpUserName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtFtpAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTempFileFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTabletAddress);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "SettingForm";
            this.Text = "设置";
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTabletAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTempFileFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFtpAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFtpUserName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtFtpPassword;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTabletPort;
    }
}
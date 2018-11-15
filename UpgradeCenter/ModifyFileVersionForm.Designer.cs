namespace UpgradeCenter
{
    partial class ModifyFileVersionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModifyFileVersionForm));
            this.label_FileNameTips = new System.Windows.Forms.Label();
            this.label_FileVersionTips = new System.Windows.Forms.Label();
            this.textBox_FileVersion = new System.Windows.Forms.TextBox();
            this.button_OK = new System.Windows.Forms.Button();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label_FileNameTips
            // 
            this.label_FileNameTips.AutoSize = true;
            this.label_FileNameTips.Location = new System.Drawing.Point(23, 30);
            this.label_FileNameTips.Name = "label_FileNameTips";
            this.label_FileNameTips.Size = new System.Drawing.Size(53, 12);
            this.label_FileNameTips.TabIndex = 1;
            this.label_FileNameTips.Text = "文件名：";
            // 
            // label_FileVersionTips
            // 
            this.label_FileVersionTips.AutoSize = true;
            this.label_FileVersionTips.Location = new System.Drawing.Point(23, 67);
            this.label_FileVersionTips.Name = "label_FileVersionTips";
            this.label_FileVersionTips.Size = new System.Drawing.Size(53, 12);
            this.label_FileVersionTips.TabIndex = 3;
            this.label_FileVersionTips.Text = "版本号：";
            // 
            // textBox_FileVersion
            // 
            this.textBox_FileVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_FileVersion.Location = new System.Drawing.Point(82, 63);
            this.textBox_FileVersion.Name = "textBox_FileVersion";
            this.textBox_FileVersion.Size = new System.Drawing.Size(236, 21);
            this.textBox_FileVersion.TabIndex = 2;
            this.textBox_FileVersion.Text = "1.0";
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(247, 106);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 4;
            this.button_OK.Text = "确认";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // comboBox
            // 
            this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(82, 26);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(236, 20);
            this.comboBox.TabIndex = 5;
            // 
            // ModifyFileVersionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 137);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.label_FileVersionTips);
            this.Controls.Add(this.textBox_FileVersion);
            this.Controls.Add(this.label_FileNameTips);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModifyFileVersionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "文件版本管理";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_FileNameTips;
        private System.Windows.Forms.Label label_FileVersionTips;
        private System.Windows.Forms.TextBox textBox_FileVersion;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.ComboBox comboBox;
    }
}
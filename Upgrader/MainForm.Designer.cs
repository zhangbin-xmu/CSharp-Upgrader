namespace Upgrader
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label_Title = new System.Windows.Forms.Label();
            this.label_File = new System.Windows.Forms.Label();
            this.label_Progress = new System.Windows.Forms.Label();
            this.label_Reconnect = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.GhostWhite;
            this.progressBar.Location = new System.Drawing.Point(12, 62);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(360, 23);
            this.progressBar.TabIndex = 1;
            // 
            // label_Title
            // 
            this.label_Title.Location = new System.Drawing.Point(12, 9);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(360, 27);
            this.label_Title.TabIndex = 2;
            this.label_Title.Text = "正在分析升级数据......";
            this.label_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_File
            // 
            this.label_File.Location = new System.Drawing.Point(12, 36);
            this.label_File.Name = "label_File";
            this.label_File.Size = new System.Drawing.Size(360, 23);
            this.label_File.TabIndex = 3;
            this.label_File.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_Progress
            // 
            this.label_Progress.Font = new System.Drawing.Font("Courier New", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Progress.ForeColor = System.Drawing.Color.Brown;
            this.label_Progress.Location = new System.Drawing.Point(230, 92);
            this.label_Progress.Name = "label_Progress";
            this.label_Progress.Size = new System.Drawing.Size(142, 23);
            this.label_Progress.TabIndex = 3;
            this.label_Progress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_Reconnect
            // 
            this.label_Reconnect.Font = new System.Drawing.Font("Courier New", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Reconnect.ForeColor = System.Drawing.Color.Red;
            this.label_Reconnect.Location = new System.Drawing.Point(11, 92);
            this.label_Reconnect.Name = "label_Reconnect";
            this.label_Reconnect.Size = new System.Drawing.Size(208, 23);
            this.label_Reconnect.TabIndex = 3;
            this.label_Reconnect.Text = "连接断开，正在重连......";
            this.label_Reconnect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_Reconnect.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 137);
            this.Controls.Add(this.label_Reconnect);
            this.Controls.Add(this.label_Progress);
            this.Controls.Add(this.label_File);
            this.Controls.Add(this.label_Title);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "升级程序";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Label label_Progress;
        private System.Windows.Forms.Label label_File;
        private System.Windows.Forms.Label label_Reconnect;

    }
}


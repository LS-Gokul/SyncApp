
namespace LSSyncApp
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.pVersionUpdate = new System.Windows.Forms.ProgressBar();
            this.lbUpdate = new System.Windows.Forms.Button();
            this.lbCancel = new System.Windows.Forms.Button();
            this.rCurrentVersion = new System.Windows.Forms.RichTextBox();
            this.rCurrentVersionB1 = new System.Windows.Forms.RichTextBox();
            this.rCurrentVersionBack = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rDesc = new System.Windows.Forms.RichTextBox();
            this.rLatestVersion = new System.Windows.Forms.RichTextBox();
            this.rLatestVersionB1 = new System.Windows.Forms.RichTextBox();
            this.rLatestVersionBack = new System.Windows.Forms.RichTextBox();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.mainPanel.Controls.Add(this.pVersionUpdate);
            this.mainPanel.Controls.Add(this.lbUpdate);
            this.mainPanel.Controls.Add(this.lbCancel);
            this.mainPanel.Controls.Add(this.rCurrentVersion);
            this.mainPanel.Controls.Add(this.rCurrentVersionB1);
            this.mainPanel.Controls.Add(this.rCurrentVersionBack);
            this.mainPanel.Controls.Add(this.label3);
            this.mainPanel.Controls.Add(this.label2);
            this.mainPanel.Controls.Add(this.label1);
            this.mainPanel.Controls.Add(this.rDesc);
            this.mainPanel.Controls.Add(this.rLatestVersion);
            this.mainPanel.Controls.Add(this.rLatestVersionB1);
            this.mainPanel.Controls.Add(this.rLatestVersionBack);
            this.mainPanel.Location = new System.Drawing.Point(60, 43);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(2059, 1137);
            this.mainPanel.TabIndex = 0;
            // 
            // pVersionUpdate
            // 
            this.pVersionUpdate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(250)))), ((int)(((byte)(159)))));
            this.pVersionUpdate.Location = new System.Drawing.Point(1102, 991);
            this.pVersionUpdate.Name = "pVersionUpdate";
            this.pVersionUpdate.Size = new System.Drawing.Size(690, 56);
            this.pVersionUpdate.TabIndex = 5;
            this.pVersionUpdate.Visible = false;
            // 
            // lbUpdate
            // 
            this.lbUpdate.BackColor = System.Drawing.Color.Transparent;
            this.lbUpdate.BackgroundImage = global::LSSyncApp.Properties.Resources.Button_Backround;
            this.lbUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbUpdate.FlatAppearance.BorderSize = 0;
            this.lbUpdate.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUpdate.ForeColor = System.Drawing.Color.White;
            this.lbUpdate.Location = new System.Drawing.Point(1805, 987);
            this.lbUpdate.Name = "lbUpdate";
            this.lbUpdate.Size = new System.Drawing.Size(183, 61);
            this.lbUpdate.TabIndex = 2;
            this.lbUpdate.Text = "Update";
            this.lbUpdate.UseVisualStyleBackColor = false;
            this.lbUpdate.Click += new System.EventHandler(this.lbUpdate_Click);
            // 
            // lbCancel
            // 
            this.lbCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.lbCancel.BackgroundImage = global::LSSyncApp.Properties.Resources.Close;
            this.lbCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.lbCancel.ForeColor = System.Drawing.Color.Black;
            this.lbCancel.Location = new System.Drawing.Point(1983, 24);
            this.lbCancel.Name = "lbCancel";
            this.lbCancel.Size = new System.Drawing.Size(52, 54);
            this.lbCancel.TabIndex = 1;
            this.lbCancel.UseVisualStyleBackColor = false;
            this.lbCancel.Click += new System.EventHandler(this.lbCancel_Click);
            // 
            // rCurrentVersion
            // 
            this.rCurrentVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.rCurrentVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rCurrentVersion.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rCurrentVersion.ForeColor = System.Drawing.Color.White;
            this.rCurrentVersion.Location = new System.Drawing.Point(57, 373);
            this.rCurrentVersion.Name = "rCurrentVersion";
            this.rCurrentVersion.Size = new System.Drawing.Size(886, 682);
            this.rCurrentVersion.TabIndex = 0;
            this.rCurrentVersion.Text = "";
            // 
            // rCurrentVersionB1
            // 
            this.rCurrentVersionB1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.rCurrentVersionB1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rCurrentVersionB1.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rCurrentVersionB1.ForeColor = System.Drawing.Color.White;
            this.rCurrentVersionB1.Location = new System.Drawing.Point(37, 353);
            this.rCurrentVersionB1.Name = "rCurrentVersionB1";
            this.rCurrentVersionB1.Size = new System.Drawing.Size(926, 722);
            this.rCurrentVersionB1.TabIndex = 2;
            this.rCurrentVersionB1.Text = "";
            // 
            // rCurrentVersionBack
            // 
            this.rCurrentVersionBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(255)))));
            this.rCurrentVersionBack.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rCurrentVersionBack.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rCurrentVersionBack.ForeColor = System.Drawing.Color.White;
            this.rCurrentVersionBack.Location = new System.Drawing.Point(36, 352);
            this.rCurrentVersionBack.Name = "rCurrentVersionBack";
            this.rCurrentVersionBack.Size = new System.Drawing.Size(928, 724);
            this.rCurrentVersionBack.TabIndex = 1000;
            this.rCurrentVersionBack.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(1074, 295);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(290, 54);
            this.label3.TabIndex = 10;
            this.label3.Text = "Latest Version";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(39, 295);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(319, 54);
            this.label2.TabIndex = 9;
            this.label2.Text = "Current Version";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(39, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 54);
            this.label1.TabIndex = 8;
            this.label1.Text = "About";
            // 
            // rDesc
            // 
            this.rDesc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.rDesc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rDesc.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rDesc.ForeColor = System.Drawing.Color.White;
            this.rDesc.Location = new System.Drawing.Point(36, 88);
            this.rDesc.Name = "rDesc";
            this.rDesc.Size = new System.Drawing.Size(1973, 190);
            this.rDesc.TabIndex = 0;
            this.rDesc.Text = "";
            // 
            // rLatestVersion
            // 
            this.rLatestVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.rLatestVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rLatestVersion.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rLatestVersion.ForeColor = System.Drawing.Color.White;
            this.rLatestVersion.Location = new System.Drawing.Point(1092, 373);
            this.rLatestVersion.Name = "rLatestVersion";
            this.rLatestVersion.Size = new System.Drawing.Size(886, 682);
            this.rLatestVersion.TabIndex = 0;
            this.rLatestVersion.Text = "";
            // 
            // rLatestVersionB1
            // 
            this.rLatestVersionB1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.rLatestVersionB1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rLatestVersionB1.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rLatestVersionB1.ForeColor = System.Drawing.Color.White;
            this.rLatestVersionB1.Location = new System.Drawing.Point(1072, 353);
            this.rLatestVersionB1.Name = "rLatestVersionB1";
            this.rLatestVersionB1.Size = new System.Drawing.Size(926, 722);
            this.rLatestVersionB1.TabIndex = 3;
            this.rLatestVersionB1.Text = "";
            // 
            // rLatestVersionBack
            // 
            this.rLatestVersionBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(255)))));
            this.rLatestVersionBack.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rLatestVersionBack.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rLatestVersionBack.ForeColor = System.Drawing.Color.White;
            this.rLatestVersionBack.Location = new System.Drawing.Point(1071, 352);
            this.rLatestVersionBack.Name = "rLatestVersionBack";
            this.rLatestVersionBack.Size = new System.Drawing.Size(928, 724);
            this.rLatestVersionBack.TabIndex = 1001;
            this.rLatestVersionBack.Text = "";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.CancelButton = this.lbCancel;
            this.ClientSize = new System.Drawing.Size(2194, 1192);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "About";
            this.Text = "About";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AboutClosing);
            this.Load += new System.EventHandler(this.About_Load);
            this.Shown += new System.EventHandler(this.ResizeEvent);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button lbUpdate;
        private System.Windows.Forms.RichTextBox rDesc;
        private System.Windows.Forms.RichTextBox rLatestVersion;
        private System.Windows.Forms.RichTextBox rCurrentVersion;
        private System.Windows.Forms.Button lbCancel;
        private System.Windows.Forms.ProgressBar pVersionUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox rCurrentVersionBack;
        private System.Windows.Forms.RichTextBox rLatestVersionBack;
        private System.Windows.Forms.RichTextBox rCurrentVersionB1;
        private System.Windows.Forms.RichTextBox rLatestVersionB1;
    }
}
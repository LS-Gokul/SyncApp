
using System;
using System.Windows.Forms;

namespace LSSyncApp
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.lbl_firm = new System.Windows.Forms.Label();
            this.comBFirm = new System.Windows.Forms.ComboBox();
            this.btnSignIn = new System.Windows.Forms.Button();
            this.btn_loginfirm = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.lbPassword = new System.Windows.Forms.Label();
            this.lbUserName = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_firm
            // 
            this.lbl_firm.AutoSize = true;
            this.lbl_firm.BackColor = System.Drawing.Color.Transparent;
            this.lbl_firm.Font = new System.Drawing.Font("Ebrima", 15.9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_firm.ForeColor = System.Drawing.Color.White;
            this.lbl_firm.Location = new System.Drawing.Point(1744, 958);
            this.lbl_firm.Name = "lbl_firm";
            this.lbl_firm.Size = new System.Drawing.Size(339, 71);
            this.lbl_firm.TabIndex = 7;
            this.lbl_firm.Text = "Choose Firm";
            // 
            // comBFirm
            // 
            this.comBFirm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comBFirm.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comBFirm.FormattingEnabled = true;
            this.comBFirm.Location = new System.Drawing.Point(1745, 1044);
            this.comBFirm.Name = "comBFirm";
            this.comBFirm.Size = new System.Drawing.Size(650, 54);
            this.comBFirm.TabIndex = 1;
            // 
            // btnSignIn
            // 
            this.btnSignIn.BackColor = System.Drawing.Color.Transparent;
            this.btnSignIn.BackgroundImage = global::LSSyncApp.Properties.Resources.Button_Backround;
            this.btnSignIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSignIn.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSignIn.ForeColor = System.Drawing.Color.White;
            this.btnSignIn.Location = new System.Drawing.Point(1916, 1288);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Size = new System.Drawing.Size(306, 84);
            this.btnSignIn.TabIndex = 9;
            this.btnSignIn.Text = "Sign In";
            this.btnSignIn.UseVisualStyleBackColor = false;
            this.btnSignIn.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_loginfirm
            // 
            this.btn_loginfirm.BackColor = System.Drawing.Color.Transparent;
            this.btn_loginfirm.BackgroundImage = global::LSSyncApp.Properties.Resources.Button_Backround;
            this.btn_loginfirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_loginfirm.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_loginfirm.ForeColor = System.Drawing.Color.White;
            this.btn_loginfirm.Location = new System.Drawing.Point(1916, 1121);
            this.btn_loginfirm.Name = "btn_loginfirm";
            this.btn_loginfirm.Size = new System.Drawing.Size(306, 84);
            this.btn_loginfirm.TabIndex = 2;
            this.btn_loginfirm.Text = "Confirm";
            this.btn_loginfirm.UseVisualStyleBackColor = false;
            this.btn_loginfirm.Click += new System.EventHandler(this.btn_loginfirm_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainPanel.BackgroundImage = global::LSSyncApp.Properties.Resources.LoginPage;
            this.mainPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mainPanel.Controls.Add(this.tbPassword);
            this.mainPanel.Controls.Add(this.tbUserName);
            this.mainPanel.Controls.Add(this.lbPassword);
            this.mainPanel.Controls.Add(this.lbUserName);
            this.mainPanel.Controls.Add(this.lbStatus);
            this.mainPanel.Controls.Add(this.lbl_firm);
            this.mainPanel.Controls.Add(this.btnSignIn);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(2648, 1423);
            this.mainPanel.TabIndex = 11;
            // 
            // tbPassword
            // 
            this.tbPassword.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPassword.Location = new System.Drawing.Point(1776, 1201);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(619, 52);
            this.tbPassword.TabIndex = 15;
            // 
            // tbUserName
            // 
            this.tbUserName.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbUserName.Location = new System.Drawing.Point(1776, 1101);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(619, 52);
            this.tbUserName.TabIndex = 14;
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.BackColor = System.Drawing.Color.Transparent;
            this.lbPassword.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPassword.ForeColor = System.Drawing.Color.White;
            this.lbPassword.Location = new System.Drawing.Point(1536, 1201);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(203, 54);
            this.lbPassword.TabIndex = 13;
            this.lbPassword.Text = "Password";
            this.lbPassword.Visible = false;
            // 
            // lbUserName
            // 
            this.lbUserName.AutoSize = true;
            this.lbUserName.BackColor = System.Drawing.Color.Transparent;
            this.lbUserName.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUserName.ForeColor = System.Drawing.Color.White;
            this.lbUserName.Location = new System.Drawing.Point(1507, 1103);
            this.lbUserName.Name = "lbUserName";
            this.lbUserName.Size = new System.Drawing.Size(232, 54);
            this.lbUserName.TabIndex = 12;
            this.lbUserName.Text = "User Name";
            this.lbUserName.Visible = false;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.BackColor = System.Drawing.Color.Transparent;
            this.lbStatus.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.ForeColor = System.Drawing.Color.White;
            this.lbStatus.Location = new System.Drawing.Point(1420, 891);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(256, 54);
            this.lbStatus.TabIndex = 10;
            this.lbStatus.Text = "Choose Firm";
            this.lbStatus.Visible = false;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.BackgroundImage = global::LSSyncApp.Properties.Resources.LoginPage;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(2648, 1423);
            this.Controls.Add(this.btn_loginfirm);
            this.Controls.Add(this.comBFirm);
            this.Controls.Add(this.mainPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Login_Load);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_loginfirm;
        private System.Windows.Forms.ComboBox comBFirm;
        private System.Windows.Forms.Label lbl_firm;
        private System.Windows.Forms.Button btnSignIn;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label lbStatus;
        private Label lbPassword;
        private Label lbUserName;
        private TextBox tbPassword;
        private TextBox tbUserName;
    }
}
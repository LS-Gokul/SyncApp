
namespace LSSyncApp.Forms
{
    partial class CompanyList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvCompanyList = new System.Windows.Forms.DataGridView();
            this.CompanyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.guid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ltIp = new System.Windows.Forms.TextBox();
            this.ltPort = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCompanyList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCompanyList
            // 
            this.dgvCompanyList.AllowUserToAddRows = false;
            this.dgvCompanyList.AllowUserToDeleteRows = false;
            this.dgvCompanyList.AllowUserToResizeRows = false;
            this.dgvCompanyList.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCompanyList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCompanyList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCompanyList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CompanyName,
            this.guid});
            this.dgvCompanyList.Location = new System.Drawing.Point(12, 160);
            this.dgvCompanyList.Name = "dgvCompanyList";
            this.dgvCompanyList.ReadOnly = true;
            this.dgvCompanyList.RowHeadersVisible = false;
            this.dgvCompanyList.RowHeadersWidth = 102;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgvCompanyList.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCompanyList.RowTemplate.Height = 40;
            this.dgvCompanyList.Size = new System.Drawing.Size(865, 477);
            this.dgvCompanyList.TabIndex = 0;
            this.dgvCompanyList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCompanyList_CellContentClick_1);
            this.dgvCompanyList.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCompanyList_CellContentClick);
            // 
            // CompanyName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CompanyName.DefaultCellStyle = dataGridViewCellStyle2;
            this.CompanyName.HeaderText = "Company Name";
            this.CompanyName.MinimumWidth = 20;
            this.CompanyName.Name = "CompanyName";
            this.CompanyName.ReadOnly = true;
            this.CompanyName.Width = 400;
            // 
            // guid
            // 
            this.guid.HeaderText = "GUID";
            this.guid.MinimumWidth = 12;
            this.guid.Name = "guid";
            this.guid.ReadOnly = true;
            this.guid.Visible = false;
            this.guid.Width = 250;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Gold;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 647);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(713, 38);
            this.label1.TabIndex = 1;
            this.label1.Text = "Note : Double Click on any Company to Choose";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(305, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(274, 38);
            this.label2.TabIndex = 2;
            this.label2.Text = "Choose Company";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 38);
            this.label3.TabIndex = 3;
            this.label3.Text = "IP";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(412, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 38);
            this.label4.TabIndex = 4;
            this.label4.Text = "Port";
            // 
            // ltIp
            // 
            this.ltIp.Location = new System.Drawing.Point(68, 26);
            this.ltIp.Name = "ltIp";
            this.ltIp.Size = new System.Drawing.Size(319, 38);
            this.ltIp.TabIndex = 5;
            this.ltIp.Text = "localhost";
            // 
            // ltPort
            // 
            this.ltPort.Location = new System.Drawing.Point(496, 27);
            this.ltPort.Name = "ltPort";
            this.ltPort.Size = new System.Drawing.Size(195, 38);
            this.ltPort.TabIndex = 6;
            this.ltPort.Text = "9000";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(719, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(146, 41);
            this.button1.TabIndex = 7;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CompanyList
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Alert;
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(889, 699);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ltPort);
            this.Controls.Add(this.ltIp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvCompanyList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompanyList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CompanyList";
            this.Load += new System.EventHandler(this.CompanyList_Load);
            this.Shown += new System.EventHandler(this.CompanyList_PostLoad);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCompanyList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCompanyList;
        private new System.Windows.Forms.DataGridViewTextBoxColumn CompanyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn guid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ltIp;
        private System.Windows.Forms.TextBox ltPort;
        private System.Windows.Forms.Button button1;
    }
}
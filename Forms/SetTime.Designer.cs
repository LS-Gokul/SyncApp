
namespace LSSyncApp.Forms
{
    partial class SetTime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetTime));
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpMinDate = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvTableList = new System.Windows.Forms.DataGridView();
            this.sel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableList)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Gold;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(782, 41);
            this.label2.TabIndex = 0;
            this.label2.Text = "Note : Please Choose the date to Sync the report from.";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 701);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1046, 61);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(60, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 41);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose Date";
            // 
            // dtpMinDate
            // 
            this.dtpMinDate.CalendarFont = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpMinDate.Font = new System.Drawing.Font("Times New Roman", 8.1F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpMinDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpMinDate.Location = new System.Drawing.Point(317, 64);
            this.dtpMinDate.MaxDate = new System.DateTime(2022, 6, 9, 0, 0, 0, 0);
            this.dtpMinDate.MinDate = new System.DateTime(2007, 1, 1, 0, 0, 0, 0);
            this.dtpMinDate.Name = "dtpMinDate";
            this.dtpMinDate.Size = new System.Drawing.Size(234, 39);
            this.dtpMinDate.TabIndex = 0;
            this.dtpMinDate.Value = new System.DateTime(2022, 6, 9, 0, 0, 0, 0);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(666, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(245, 50);
            this.button1.TabIndex = 3;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvTableList);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.dtpMinDate);
            this.panel2.Controls.Add(this.splitter1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1046, 762);
            this.panel2.TabIndex = 4;
            // 
            // dgvTableList
            // 
            this.dgvTableList.AllowUserToAddRows = false;
            this.dgvTableList.AllowUserToDeleteRows = false;
            this.dgvTableList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableList.ColumnHeadersVisible = false;
            this.dgvTableList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sel,
            this.tableName,
            this.tbl});
            this.dgvTableList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTableList.Location = new System.Drawing.Point(0, 148);
            this.dgvTableList.Name = "dgvTableList";
            this.dgvTableList.RowHeadersVisible = false;
            this.dgvTableList.RowHeadersWidth = 102;
            this.dgvTableList.RowTemplate.Height = 40;
            this.dgvTableList.Size = new System.Drawing.Size(1046, 614);
            this.dgvTableList.TabIndex = 5;
            // 
            // sel
            // 
            this.sel.HeaderText = "sel";
            this.sel.MinimumWidth = 12;
            this.sel.Name = "sel";
            this.sel.Width = 250;
            // 
            // tableName
            // 
            this.tableName.HeaderText = "Table";
            this.tableName.MinimumWidth = 12;
            this.tableName.Name = "tableName";
            this.tableName.ReadOnly = true;
            this.tableName.Width = 250;
            // 
            // tbl
            // 
            this.tbl.HeaderText = "tbl";
            this.tbl.MinimumWidth = 12;
            this.tbl.Name = "tbl";
            this.tbl.Visible = false;
            this.tbl.Width = 250;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1046, 148);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // SetTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1046, 762);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetTime";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Time";
            this.Load += new System.EventHandler(this.SetTime_Load);
            this.Shown += new System.EventHandler(this.SetTime_PostLoad);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpMinDate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvTableList;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn sel;
        private System.Windows.Forms.DataGridViewTextBoxColumn tableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn tbl;
    }
}
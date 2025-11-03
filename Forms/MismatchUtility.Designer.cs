
namespace LSSyncApp.Forms
{
    partial class MismatchUtility
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MismatchUtility));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvTableList = new System.Windows.Forms.DataGridView();
            this.table = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tblCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dest = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnList = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pTableList = new System.Windows.Forms.Panel();
            this.lTableList = new System.Windows.Forms.Label();
            this.dgvMismatchList = new System.Windows.Forms.DataGridView();
            this.pMismatchList = new System.Windows.Forms.Panel();
            this.lpbGlobalPB = new System.Windows.Forms.ProgressBar();
            this.lbSync = new System.Windows.Forms.Button();
            this.lMismatchList = new System.Windows.Forms.Label();
            this.topPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxSourceList = new System.Windows.Forms.ComboBox();
            this.lbCancel = new System.Windows.Forms.Button();
            this.wbLoader = new System.Windows.Forms.WebBrowser();
            this.bgProcess = new System.ComponentModel.BackgroundWorker();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableList)).BeginInit();
            this.pTableList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMismatchList)).BeginInit();
            this.pMismatchList.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.splitContainer1);
            this.mainPanel.Controls.Add(this.topPanel);
            this.mainPanel.Location = new System.Drawing.Point(21, 29);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1510, 812);
            this.mainPanel.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 73);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvTableList);
            this.splitContainer1.Panel1.Controls.Add(this.pTableList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvMismatchList);
            this.splitContainer1.Panel2.Controls.Add(this.pMismatchList);
            this.splitContainer1.Size = new System.Drawing.Size(1510, 739);
            this.splitContainer1.SplitterDistance = 503;
            this.splitContainer1.TabIndex = 1;
            // 
            // dgvTableList
            // 
            this.dgvTableList.AllowUserToAddRows = false;
            this.dgvTableList.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTableList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTableList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.table,
            this.tblCode,
            this.source,
            this.dest,
            this.columnList,
            this.param});
            this.dgvTableList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTableList.Location = new System.Drawing.Point(0, 69);
            this.dgvTableList.Name = "dgvTableList";
            this.dgvTableList.ReadOnly = true;
            this.dgvTableList.RowHeadersVisible = false;
            this.dgvTableList.RowHeadersWidth = 102;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvTableList.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTableList.RowTemplate.Height = 40;
            this.dgvTableList.Size = new System.Drawing.Size(503, 670);
            this.dgvTableList.TabIndex = 1;
            this.dgvTableList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTableList_CellContentClick);
            // 
            // table
            // 
            this.table.HeaderText = "Table";
            this.table.MinimumWidth = 12;
            this.table.Name = "table";
            this.table.ReadOnly = true;
            this.table.Width = 250;
            // 
            // tblCode
            // 
            this.tblCode.HeaderText = "tblCode";
            this.tblCode.MinimumWidth = 12;
            this.tblCode.Name = "tblCode";
            this.tblCode.ReadOnly = true;
            this.tblCode.Visible = false;
            this.tblCode.Width = 250;
            // 
            // source
            // 
            this.source.HeaderText = "source";
            this.source.MinimumWidth = 12;
            this.source.Name = "source";
            this.source.ReadOnly = true;
            this.source.Visible = false;
            this.source.Width = 250;
            // 
            // dest
            // 
            this.dest.HeaderText = "dest";
            this.dest.MinimumWidth = 12;
            this.dest.Name = "dest";
            this.dest.ReadOnly = true;
            this.dest.Visible = false;
            this.dest.Width = 250;
            // 
            // columnList
            // 
            this.columnList.HeaderText = "columnList";
            this.columnList.MinimumWidth = 12;
            this.columnList.Name = "columnList";
            this.columnList.ReadOnly = true;
            this.columnList.Visible = false;
            this.columnList.Width = 250;
            // 
            // param
            // 
            this.param.HeaderText = "param";
            this.param.MinimumWidth = 12;
            this.param.Name = "param";
            this.param.ReadOnly = true;
            this.param.Visible = false;
            this.param.Width = 250;
            // 
            // pTableList
            // 
            this.pTableList.Controls.Add(this.lTableList);
            this.pTableList.Dock = System.Windows.Forms.DockStyle.Top;
            this.pTableList.Location = new System.Drawing.Point(0, 0);
            this.pTableList.Name = "pTableList";
            this.pTableList.Size = new System.Drawing.Size(503, 69);
            this.pTableList.TabIndex = 0;
            // 
            // lTableList
            // 
            this.lTableList.AutoSize = true;
            this.lTableList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lTableList.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTableList.Location = new System.Drawing.Point(0, 0);
            this.lTableList.Name = "lTableList";
            this.lTableList.Size = new System.Drawing.Size(171, 46);
            this.lTableList.TabIndex = 0;
            this.lTableList.Text = "Table List";
            // 
            // dgvMismatchList
            // 
            this.dgvMismatchList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMismatchList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMismatchList.Location = new System.Drawing.Point(0, 69);
            this.dgvMismatchList.Name = "dgvMismatchList";
            this.dgvMismatchList.RowHeadersVisible = false;
            this.dgvMismatchList.RowHeadersWidth = 102;
            this.dgvMismatchList.RowTemplate.Height = 40;
            this.dgvMismatchList.Size = new System.Drawing.Size(1003, 670);
            this.dgvMismatchList.TabIndex = 3;
            this.dgvMismatchList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMismatchList_CellContentClick);
            this.dgvMismatchList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMismatchList_CellValueChanged);
            // 
            // pMismatchList
            // 
            this.pMismatchList.Controls.Add(this.lpbGlobalPB);
            this.pMismatchList.Controls.Add(this.lbSync);
            this.pMismatchList.Controls.Add(this.lMismatchList);
            this.pMismatchList.Dock = System.Windows.Forms.DockStyle.Top;
            this.pMismatchList.Location = new System.Drawing.Point(0, 0);
            this.pMismatchList.Name = "pMismatchList";
            this.pMismatchList.Size = new System.Drawing.Size(1003, 69);
            this.pMismatchList.TabIndex = 1;
            // 
            // lpbGlobalPB
            // 
            this.lpbGlobalPB.Dock = System.Windows.Forms.DockStyle.Left;
            this.lpbGlobalPB.ForeColor = System.Drawing.Color.LightGreen;
            this.lpbGlobalPB.Location = new System.Drawing.Point(307, 0);
            this.lpbGlobalPB.Name = "lpbGlobalPB";
            this.lpbGlobalPB.Size = new System.Drawing.Size(550, 69);
            this.lpbGlobalPB.TabIndex = 9;
            // 
            // lbSync
            // 
            this.lbSync.BackgroundImage = global::LSSyncApp.Properties.Resources.Button_Backround;
            this.lbSync.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbSync.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbSync.Enabled = false;
            this.lbSync.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSync.ForeColor = System.Drawing.Color.White;
            this.lbSync.Location = new System.Drawing.Point(94, 0);
            this.lbSync.Name = "lbSync";
            this.lbSync.Size = new System.Drawing.Size(213, 69);
            this.lbSync.TabIndex = 8;
            this.lbSync.Text = "Sync Data";
            this.lbSync.UseVisualStyleBackColor = true;
            this.lbSync.Click += new System.EventHandler(this.lbSync_Click);
            // 
            // lMismatchList
            // 
            this.lMismatchList.AutoSize = true;
            this.lMismatchList.Dock = System.Windows.Forms.DockStyle.Left;
            this.lMismatchList.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Bold);
            this.lMismatchList.Location = new System.Drawing.Point(0, 0);
            this.lMismatchList.Name = "lMismatchList";
            this.lMismatchList.Size = new System.Drawing.Size(94, 46);
            this.lMismatchList.TabIndex = 7;
            this.lMismatchList.Text = "Data";
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.label3);
            this.topPanel.Controls.Add(this.cbxSourceList);
            this.topPanel.Controls.Add(this.lbCancel);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1510, 73);
            this.topPanel.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 46);
            this.label3.TabIndex = 8;
            this.label3.Text = "Source";
            // 
            // cbxSourceList
            // 
            this.cbxSourceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSourceList.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxSourceList.FormattingEnabled = true;
            this.cbxSourceList.Location = new System.Drawing.Point(154, 11);
            this.cbxSourceList.Name = "cbxSourceList";
            this.cbxSourceList.Size = new System.Drawing.Size(640, 53);
            this.cbxSourceList.TabIndex = 7;
            // 
            // lbCancel
            // 
            this.lbCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.lbCancel.BackgroundImage = global::LSSyncApp.Properties.Resources.Close1;
            this.lbCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.lbCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbCancel.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCancel.Location = new System.Drawing.Point(1445, 0);
            this.lbCancel.Name = "lbCancel";
            this.lbCancel.Size = new System.Drawing.Size(65, 73);
            this.lbCancel.TabIndex = 6;
            this.lbCancel.UseVisualStyleBackColor = false;
            this.lbCancel.Click += new System.EventHandler(this.lbCancel_Click);
            // 
            // wbLoader
            // 
            this.wbLoader.Location = new System.Drawing.Point(1569, 181);
            this.wbLoader.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbLoader.Name = "wbLoader";
            this.wbLoader.Size = new System.Drawing.Size(214, 160);
            this.wbLoader.TabIndex = 1;
            // 
            // MismatchUtility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.CancelButton = this.lbCancel;
            this.ClientSize = new System.Drawing.Size(1847, 998);
            this.Controls.Add(this.wbLoader);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MismatchUtility";
            this.Text = "Mismatch Utility";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MisMatchClosing);
            this.Shown += new System.EventHandler(this.Post_Load);
            this.mainPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableList)).EndInit();
            this.pTableList.ResumeLayout(false);
            this.pTableList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMismatchList)).EndInit();
            this.pMismatchList.ResumeLayout(false);
            this.pMismatchList.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pTableList;
        private System.Windows.Forms.Panel pMismatchList;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lTableList;
        private System.Windows.Forms.DataGridView dgvMismatchList;
        private System.Windows.Forms.Label lMismatchList;
        private System.Windows.Forms.DataGridView dgvTableList;
        private System.Windows.Forms.Button lbCancel;
        private System.Windows.Forms.WebBrowser wbLoader;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxSourceList;
        private System.ComponentModel.BackgroundWorker bgProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn table;
        private System.Windows.Forms.DataGridViewTextBoxColumn tblCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn source;
        private System.Windows.Forms.DataGridViewTextBoxColumn dest;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnList;
        private System.Windows.Forms.Button lbSync;
        private System.Windows.Forms.ProgressBar lpbGlobalPB;
        private System.Windows.Forms.DataGridViewTextBoxColumn param;
    }
}
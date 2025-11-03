
namespace LSSyncApp.Forms
{
    partial class SyncData
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SyncData));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.dgvTest = new System.Windows.Forms.DataGridView();
            this.gbType = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lbCancel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rbReportWise = new System.Windows.Forms.RadioButton();
            this.rbModuleWise = new System.Windows.Forms.RadioButton();
            this.dgvModule = new System.Windows.Forms.DataGridView();
            this.Report = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Progress = new LSSyncApp.Controllers.DataGridViewProgressColumn();
            this.SyncButton = new LSSyncApp.Controllers.DataGridViewDisableButtonColumn();
            this.CancelButton = new LSSyncApp.Controllers.DataGridViewDisableButtonColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Seq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Params = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rptCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dataGridViewButtonColumn2 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dataGridViewProgressColumn1 = new LSSyncApp.Controllers.DataGridViewProgressColumn();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTest)).BeginInit();
            this.gbType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvModule)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.mainPanel.Controls.Add(this.dgvTest);
            this.mainPanel.Controls.Add(this.gbType);
            this.mainPanel.Controls.Add(this.dgvModule);
            this.mainPanel.Location = new System.Drawing.Point(62, 62);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(2204, 974);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mainPanel_Paint);
            // 
            // dgvTest
            // 
            this.dgvTest.AllowUserToAddRows = false;
            this.dgvTest.AllowUserToDeleteRows = false;
            this.dgvTest.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTest.Location = new System.Drawing.Point(22, 132);
            this.dgvTest.Name = "dgvTest";
            this.dgvTest.ReadOnly = true;
            this.dgvTest.RowHeadersVisible = false;
            this.dgvTest.RowHeadersWidth = 102;
            this.dgvTest.RowTemplate.Height = 40;
            this.dgvTest.Size = new System.Drawing.Size(240, 150);
            this.dgvTest.TabIndex = 5;
            this.dgvTest.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // gbType
            // 
            this.gbType.Controls.Add(this.label1);
            this.gbType.Controls.Add(this.comboBox1);
            this.gbType.Controls.Add(this.lbCancel);
            this.gbType.Controls.Add(this.button1);
            this.gbType.Controls.Add(this.progressBar1);
            this.gbType.Controls.Add(this.rbReportWise);
            this.gbType.Controls.Add(this.rbModuleWise);
            this.gbType.Location = new System.Drawing.Point(21, 26);
            this.gbType.Name = "gbType";
            this.gbType.Size = new System.Drawing.Size(1956, 94);
            this.gbType.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 32);
            this.label1.TabIndex = 6;
            this.label1.Text = "Sync Days";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "3",
            "7",
            "10",
            "20",
            "30",
            "40"});
            this.comboBox1.Location = new System.Drawing.Point(183, 21);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 53);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // lbCancel
            // 
            this.lbCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(97)))));
            this.lbCancel.BackgroundImage = global::LSSyncApp.Properties.Resources.Close1;
            this.lbCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.lbCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbCancel.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCancel.Location = new System.Drawing.Point(1891, 0);
            this.lbCancel.Name = "lbCancel";
            this.lbCancel.Size = new System.Drawing.Size(65, 94);
            this.lbCancel.TabIndex = 5;
            this.lbCancel.UseVisualStyleBackColor = false;
            this.lbCancel.Click += new System.EventHandler(this.lbCancel_Click);
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::LSSyncApp.Properties.Resources.Button_Backround;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(1173, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(391, 72);
            this.button1.TabIndex = 3;
            this.button1.Text = "Create Scheduler";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(1590, 20);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(292, 45);
            this.progressBar1.TabIndex = 4;
            // 
            // rbReportWise
            // 
            this.rbReportWise.AutoSize = true;
            this.rbReportWise.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbReportWise.ForeColor = System.Drawing.Color.DodgerBlue;
            this.rbReportWise.Location = new System.Drawing.Point(847, 25);
            this.rbReportWise.Name = "rbReportWise";
            this.rbReportWise.Size = new System.Drawing.Size(234, 42);
            this.rbReportWise.TabIndex = 2;
            this.rbReportWise.TabStop = true;
            this.rbReportWise.Text = "Report Wise";
            this.rbReportWise.UseVisualStyleBackColor = true;
            this.rbReportWise.CheckedChanged += new System.EventHandler(this.rbReportWise_CheckedChanged);
            // 
            // rbModuleWise
            // 
            this.rbModuleWise.AutoSize = true;
            this.rbModuleWise.Font = new System.Drawing.Font("Times New Roman", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModuleWise.ForeColor = System.Drawing.Color.DodgerBlue;
            this.rbModuleWise.Location = new System.Drawing.Point(417, 25);
            this.rbModuleWise.Name = "rbModuleWise";
            this.rbModuleWise.Size = new System.Drawing.Size(243, 42);
            this.rbModuleWise.TabIndex = 1;
            this.rbModuleWise.TabStop = true;
            this.rbModuleWise.Text = "Module Wise";
            this.rbModuleWise.UseVisualStyleBackColor = true;
            this.rbModuleWise.CheckedChanged += new System.EventHandler(this.rbModuleWise_CheckedChanged);
            // 
            // dgvModule
            // 
            this.dgvModule.AllowUserToAddRows = false;
            this.dgvModule.AllowUserToDeleteRows = false;
            this.dgvModule.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvModule.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvModule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvModule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Report,
            this.Progress,
            this.SyncButton,
            this.CancelButton,
            this.Status,
            this.Sel,
            this.Seq,
            this.Params,
            this.rptCode});
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Times New Roman", 10F);
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvModule.DefaultCellStyle = dataGridViewCellStyle10;
            this.dgvModule.Location = new System.Drawing.Point(1640, 179);
            this.dgvModule.Name = "dgvModule";
            this.dgvModule.ReadOnly = true;
            this.dgvModule.RowHeadersVisible = false;
            this.dgvModule.RowHeadersWidth = 102;
            this.dgvModule.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Transparent;
            this.dgvModule.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvModule.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Transparent;
            this.dgvModule.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvModule.RowTemplate.Height = 40;
            this.dgvModule.Size = new System.Drawing.Size(161, 196);
            this.dgvModule.TabIndex = 0;
            // 
            // Report
            // 
            this.Report.HeaderText = "Report";
            this.Report.MinimumWidth = 12;
            this.Report.Name = "Report";
            this.Report.ReadOnly = true;
            this.Report.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Report.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Report.Width = 800;
            // 
            // Progress
            // 
            this.Progress.DataPropertyName = "Progress";
            this.Progress.HeaderText = "Progress";
            this.Progress.MinimumWidth = 12;
            this.Progress.Name = "Progress";
            this.Progress.ReadOnly = true;
            this.Progress.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Progress.Width = 400;
            // 
            // SyncButton
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Times New Roman", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
            this.SyncButton.DefaultCellStyle = dataGridViewCellStyle8;
            this.SyncButton.HeaderText = "";
            this.SyncButton.MinimumWidth = 12;
            this.SyncButton.Name = "SyncButton";
            this.SyncButton.ReadOnly = true;
            this.SyncButton.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SyncButton.Text = "Sync";
            this.SyncButton.ToolTipText = "Sync";
            this.SyncButton.Width = 150;
            // 
            // CancelButton
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Times New Roman", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.Transparent;
            this.CancelButton.DefaultCellStyle = dataGridViewCellStyle9;
            this.CancelButton.HeaderText = "";
            this.CancelButton.MinimumWidth = 12;
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.ReadOnly = true;
            this.CancelButton.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.ToolTipText = "Cancel";
            this.CancelButton.Width = 150;
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.MinimumWidth = 12;
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Status.Width = 300;
            // 
            // Sel
            // 
            this.Sel.FalseValue = "0";
            this.Sel.HeaderText = "";
            this.Sel.MinimumWidth = 12;
            this.Sel.Name = "Sel";
            this.Sel.ReadOnly = true;
            this.Sel.TrueValue = "1";
            this.Sel.Visible = false;
            this.Sel.Width = 50;
            // 
            // Seq
            // 
            this.Seq.HeaderText = "Seq";
            this.Seq.MinimumWidth = 12;
            this.Seq.Name = "Seq";
            this.Seq.ReadOnly = true;
            this.Seq.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Seq.Visible = false;
            this.Seq.Width = 50;
            // 
            // Params
            // 
            this.Params.HeaderText = "Params";
            this.Params.MinimumWidth = 12;
            this.Params.Name = "Params";
            this.Params.ReadOnly = true;
            this.Params.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Params.Visible = false;
            this.Params.Width = 250;
            // 
            // rptCode
            // 
            this.rptCode.HeaderText = "rptCode";
            this.rptCode.MinimumWidth = 12;
            this.rptCode.Name = "rptCode";
            this.rptCode.ReadOnly = true;
            this.rptCode.Visible = false;
            this.rptCode.Width = 250;
            // 
            // dataGridViewButtonColumn1
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Times New Roman", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewButtonColumn1.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewButtonColumn1.HeaderText = "Sync";
            this.dataGridViewButtonColumn1.MinimumWidth = 12;
            this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            this.dataGridViewButtonColumn1.ReadOnly = true;
            this.dataGridViewButtonColumn1.Text = "Sync";
            this.dataGridViewButtonColumn1.ToolTipText = "Sync";
            this.dataGridViewButtonColumn1.UseColumnTextForButtonValue = true;
            this.dataGridViewButtonColumn1.Width = 150;
            // 
            // dataGridViewButtonColumn2
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Times New Roman", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewButtonColumn2.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridViewButtonColumn2.HeaderText = "Cancel";
            this.dataGridViewButtonColumn2.MinimumWidth = 12;
            this.dataGridViewButtonColumn2.Name = "dataGridViewButtonColumn2";
            this.dataGridViewButtonColumn2.ReadOnly = true;
            this.dataGridViewButtonColumn2.Text = "Cancel";
            this.dataGridViewButtonColumn2.ToolTipText = "Cancel";
            this.dataGridViewButtonColumn2.UseColumnTextForButtonValue = true;
            this.dataGridViewButtonColumn2.Width = 150;
            // 
            // dataGridViewProgressColumn1
            // 
            this.dataGridViewProgressColumn1.DataPropertyName = "Progress";
            this.dataGridViewProgressColumn1.HeaderText = "Progress";
            this.dataGridViewProgressColumn1.MinimumWidth = 12;
            this.dataGridViewProgressColumn1.Name = "dataGridViewProgressColumn1";
            this.dataGridViewProgressColumn1.ReadOnly = true;
            this.dataGridViewProgressColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewProgressColumn1.Width = 400;
            // 
            // SyncData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(75)))));
            this.ClientSize = new System.Drawing.Size(2313, 1073);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SyncData";
            this.Text = "Sync";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AboutClosing);
            this.Load += new System.EventHandler(this.SyncData_Load);
            this.Shown += new System.EventHandler(this.SyncData_PostLoad);
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTest)).EndInit();
            this.gbType.ResumeLayout(false);
            this.gbType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvModule)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.DataGridView dgvModule;
        private Controllers.DataGridViewProgressColumn dataGridViewProgressColumn1;
        private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn1;
        private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn2;
        private System.Windows.Forms.RadioButton rbReportWise;
        private System.Windows.Forms.RadioButton rbModuleWise;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button lbCancel;
        private System.Windows.Forms.Panel gbType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Report;
        private Controllers.DataGridViewProgressColumn Progress;
        private Controllers.DataGridViewDisableButtonColumn SyncButton;
        private Controllers.DataGridViewDisableButtonColumn CancelButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Sel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Seq;
        private System.Windows.Forms.DataGridViewTextBoxColumn Params;
        private System.Windows.Forms.DataGridViewTextBoxColumn rptCode;
        private System.Windows.Forms.DataGridView dgvTest;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
    }
}
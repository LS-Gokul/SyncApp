namespace LSSyncApp.Forms
{
    partial class SyncScheduler
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SyncScheduler));
            this.lbCancel = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvSchedule = new System.Windows.Forms.DataGridView();
            this.lbSave = new System.Windows.Forms.Button();
            this.lbEdit = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSchedule)).BeginInit();
            this.SuspendLayout();
            // 
            // lbCancel
            // 
            this.lbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.lbCancel.Location = new System.Drawing.Point(913, 472);
            this.lbCancel.Name = "lbCancel";
            this.lbCancel.Size = new System.Drawing.Size(81, 45);
            this.lbCancel.TabIndex = 0;
            this.lbCancel.Text = "Cancel";
            this.lbCancel.UseVisualStyleBackColor = true;
            this.lbCancel.Click += new System.EventHandler(this.lbCancel_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.splitContainer1);
            this.mainPanel.Location = new System.Drawing.Point(89, 81);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1440, 992);
            this.mainPanel.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbSave);
            this.splitContainer1.Panel1.Controls.Add(this.lbEdit);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvSchedule);
            this.splitContainer1.Size = new System.Drawing.Size(1440, 992);
            this.splitContainer1.SplitterDistance = 104;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgvSchedule
            // 
            this.dgvSchedule.AllowUserToAddRows = false;
            this.dgvSchedule.AllowUserToDeleteRows = false;
            this.dgvSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSchedule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSchedule.Location = new System.Drawing.Point(0, 0);
            this.dgvSchedule.Name = "dgvSchedule";
            this.dgvSchedule.RowHeadersVisible = false;
            this.dgvSchedule.RowHeadersWidth = 102;
            this.dgvSchedule.RowTemplate.Height = 40;
            this.dgvSchedule.Size = new System.Drawing.Size(1440, 884);
            this.dgvSchedule.TabIndex = 0;
            this.dgvSchedule.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSchedule_CellContentClick);
            // 
            // lbSave
            // 
            this.lbSave.BackgroundImage = global::LSSyncApp.Properties.Resources.Button_Backround;
            this.lbSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbSave.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSave.ForeColor = System.Drawing.Color.White;
            this.lbSave.Location = new System.Drawing.Point(513, 17);
            this.lbSave.Name = "lbSave";
            this.lbSave.Size = new System.Drawing.Size(225, 62);
            this.lbSave.TabIndex = 1;
            this.lbSave.Text = "Save";
            this.lbSave.UseVisualStyleBackColor = true;
            this.lbSave.Click += new System.EventHandler(this.lbSave_Click);
            // 
            // lbEdit
            // 
            this.lbEdit.BackgroundImage = global::LSSyncApp.Properties.Resources.ButtonOrange;
            this.lbEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbEdit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.lbEdit.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbEdit.Location = new System.Drawing.Point(257, 17);
            this.lbEdit.Name = "lbEdit";
            this.lbEdit.Size = new System.Drawing.Size(225, 62);
            this.lbEdit.TabIndex = 0;
            this.lbEdit.Text = "Edit";
            this.lbEdit.UseVisualStyleBackColor = true;
            this.lbEdit.Click += new System.EventHandler(this.lbEdit_Click);
            // 
            // SyncScheduler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.lbCancel;
            this.ClientSize = new System.Drawing.Size(1613, 1136);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.lbCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SyncScheduler";
            this.Text = "Sync Scheduler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SyncSchedulerClosing);
            this.Load += new System.EventHandler(this.SyncScheduler_Load);
            this.Shown += new System.EventHandler(this.SyncScheduler_PostLoad);
            this.Resize += new System.EventHandler(this.SyncScheduler_Resize);
            this.mainPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSchedule)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button lbCancel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvSchedule;
        private System.Windows.Forms.Button lbSave;
        private System.Windows.Forms.Button lbEdit;
    }
}

namespace LSSyncApp.Forms
{
    partial class Schedulers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Schedulers));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lNote = new System.Windows.Forms.Label();
            this.dgvScheduleList = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScheduleList)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1437, 100);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Ebrima", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(1199, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(220, 66);
            this.button1.TabIndex = 0;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lNote);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 759);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1437, 67);
            this.panel2.TabIndex = 1;
            // 
            // lNote
            // 
            this.lNote.AutoSize = true;
            this.lNote.BackColor = System.Drawing.Color.Gold;
            this.lNote.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lNote.ForeColor = System.Drawing.Color.Brown;
            this.lNote.Location = new System.Drawing.Point(-6, 11);
            this.lNote.Name = "lNote";
            this.lNote.Size = new System.Drawing.Size(1367, 41);
            this.lNote.TabIndex = 1000;
            this.lNote.Text = "Note : If you want to change the sync Interval please choose the Interval in \"New" +
    " Interval\" field.";
            // 
            // dgvScheduleList
            // 
            this.dgvScheduleList.AllowUserToAddRows = false;
            this.dgvScheduleList.AllowUserToDeleteRows = false;
            this.dgvScheduleList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScheduleList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvScheduleList.Location = new System.Drawing.Point(0, 100);
            this.dgvScheduleList.Name = "dgvScheduleList";
            this.dgvScheduleList.RowHeadersVisible = false;
            this.dgvScheduleList.RowHeadersWidth = 102;
            this.dgvScheduleList.RowTemplate.DividerHeight = 3;
            this.dgvScheduleList.RowTemplate.Height = 40;
            this.dgvScheduleList.Size = new System.Drawing.Size(1437, 659);
            this.dgvScheduleList.TabIndex = 2;
            // 
            // Schedulers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1437, 826);
            this.Controls.Add(this.dgvScheduleList);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Schedulers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scheduler";
            this.Load += new System.EventHandler(this.Scheduler_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScheduleList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvScheduleList;
        private System.Windows.Forms.Label lNote;
    }
}
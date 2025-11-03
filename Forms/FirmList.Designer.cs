
namespace LSSyncApp.Forms
{
    partial class FirmList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirmList));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.bClose = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.firmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wbLoader = new System.Windows.Forms.WebBrowser();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.bClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1265, 79);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Ebrima", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(350, 54);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose Company";
            // 
            // bClose
            // 
            this.bClose.BackgroundImage = global::LSSyncApp.Properties.Resources.Close1;
            this.bClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.bClose.Location = new System.Drawing.Point(1189, 0);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(76, 79);
            this.bClose.TabIndex = 0;
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 541);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1265, 58);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Yellow;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(568, 32);
            this.label2.TabIndex = 1001;
            this.label2.Text = "Note : Double Click to Choose the Company";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.firmName,
            this.firmCode});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 79);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 102;
            this.dataGridView1.RowTemplate.Height = 40;
            this.dataGridView1.Size = new System.Drawing.Size(1265, 462);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // firmName
            // 
            this.firmName.HeaderText = "firmName";
            this.firmName.MinimumWidth = 12;
            this.firmName.Name = "firmName";
            this.firmName.ReadOnly = true;
            this.firmName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.firmName.Width = 250;
            // 
            // firmCode
            // 
            this.firmCode.HeaderText = "firmCode";
            this.firmCode.MinimumWidth = 12;
            this.firmCode.Name = "firmCode";
            this.firmCode.ReadOnly = true;
            this.firmCode.Visible = false;
            this.firmCode.Width = 250;
            // 
            // wbLoader
            // 
            this.wbLoader.Location = new System.Drawing.Point(637, 149);
            this.wbLoader.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbLoader.Name = "wbLoader";
            this.wbLoader.Size = new System.Drawing.Size(250, 250);
            this.wbLoader.TabIndex = 3;
            // 
            // FirmList
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Alert;
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1265, 599);
            this.Controls.Add(this.wbLoader);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirmList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose Company";
            this.Load += new System.EventHandler(this.FirmList_Load);
            this.Shown += new System.EventHandler(this.FirmList_PostLoad);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmCode;
        private System.Windows.Forms.WebBrowser wbLoader;
    }
}
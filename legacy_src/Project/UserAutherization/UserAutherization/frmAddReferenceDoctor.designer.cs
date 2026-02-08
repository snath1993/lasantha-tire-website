namespace UserAutherization
{
    partial class frmAddReferenceDoctor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddReferenceDoctor));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDoctName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.cmbDoctType = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDoctType)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDoctName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.cmbDoctType);
            this.groupBox1.Location = new System.Drawing.Point(5, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(497, 151);
            this.groupBox1.TabIndex = 64;
            this.groupBox1.TabStop = false;
            // 
            // txtDoctName
            // 
            this.txtDoctName.Enabled = false;
            this.txtDoctName.Location = new System.Drawing.Point(119, 80);
            this.txtDoctName.Name = "txtDoctName";
            this.txtDoctName.Size = new System.Drawing.Size(372, 20);
            this.txtDoctName.TabIndex = 150;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(18, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 16);
            this.label1.TabIndex = 149;
            this.label1.Text = "Doctor Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(18, 55);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(40, 16);
            this.label17.TabIndex = 148;
            this.label17.Text = "Type";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbDoctType
            // 
            this.cmbDoctType.CheckedListSettings.CheckStateMember = "";
            this.cmbDoctType.Enabled = false;
            this.cmbDoctType.Location = new System.Drawing.Point(119, 49);
            this.cmbDoctType.Name = "cmbDoctType";
            this.cmbDoctType.Size = new System.Drawing.Size(147, 22);
            this.cmbDoctType.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClose,
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(539, 25);
            this.toolStrip1.TabIndex = 65;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnClose
            // 
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(56, 22);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(51, 22);
            this.toolStripButton1.Text = "New";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(81, 22);
            this.toolStripButton2.Text = "Save/Print";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // frmAddReferenceDoctor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 183);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAddReferenceDoctor";
            this.Text = "Add Refered Doctors";
            this.Load += new System.EventHandler(this.frmAddReferenceDoctor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDoctType)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnClose;
        public System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtDoctName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label17;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbDoctType;
    }
}
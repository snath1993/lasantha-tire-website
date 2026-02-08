namespace UserAutherization
{
    partial class frmPationReport
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtrecept = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dgvPationReport = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPationReport)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtrecept);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Location = new System.Drawing.Point(17, 201);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(789, 49);
            this.groupBox5.TabIndex = 87;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Select Receipt No";
            // 
            // txtrecept
            // 
            this.txtrecept.Location = new System.Drawing.Point(116, 20);
            this.txtrecept.Name = "txtrecept";
            this.txtrecept.Size = new System.Drawing.Size(404, 22);
            this.txtrecept.TabIndex = 65;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 23);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 17);
            this.label3.TabIndex = 63;
            this.label3.Text = "Receipt No";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtContact);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(17, 146);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(789, 49);
            this.groupBox4.TabIndex = 86;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Select contact Number";
            // 
            // txtContact
            // 
            this.txtContact.Location = new System.Drawing.Point(116, 24);
            this.txtContact.Name = "txtContact";
            this.txtContact.Size = new System.Drawing.Size(404, 22);
            this.txtContact.TabIndex = 65;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 17);
            this.label1.TabIndex = 61;
            this.label1.Text = "Contact No";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtName);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(17, 91);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(789, 49);
            this.groupBox3.TabIndex = 85;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select Customer ";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(116, 20);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(404, 22);
            this.txtName.TabIndex = 64;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 23);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 63;
            this.label2.Text = "Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dtpFromDate);
            this.groupBox1.Controls.Add(this.dtpToDate);
            this.groupBox1.Location = new System.Drawing.Point(17, 36);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(789, 49);
            this.groupBox1.TabIndex = 84;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Date Range";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 28);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "From Date";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(333, 27);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 1;
            this.label5.Text = "To Date";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(116, 23);
            this.dtpFromDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(196, 22);
            this.dtpFromDate.TabIndex = 3;
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(415, 22);
            this.dtpToDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(201, 22);
            this.dtpToDate.TabIndex = 4;
            // 
            // dgvPationReport
            // 
            this.dgvPationReport.AllowUserToAddRows = false;
            this.dgvPationReport.AllowUserToDeleteRows = false;
            this.dgvPationReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPationReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.Column1,
            this.Column3,
            this.Column4});
            this.dgvPationReport.Location = new System.Drawing.Point(5, 264);
            this.dgvPationReport.Name = "dgvPationReport";
            this.dgvPationReport.ReadOnly = true;
            this.dgvPationReport.RowHeadersVisible = false;
            this.dgvPationReport.RowTemplate.Height = 24;
            this.dgvPationReport.Size = new System.Drawing.Size(846, 263);
            this.dgvPationReport.TabIndex = 83;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Gainsboro;
            this.toolStrip1.BackgroundImage = global::UserAutherization.Properties.Resources.BBar;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClose,
            this.btnPrint,
            this.btnLoad});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(854, 27);
            this.toolStrip1.TabIndex = 88;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnClose
            // 
            this.btnClose.Image = global::UserAutherization.Properties.Resources.close1;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(69, 24);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Image = global::UserAutherization.Properties.Resources.rinticon;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(63, 24);
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Image = global::UserAutherization.Properties.Resources.supReturns;
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(66, 24);
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Receipt No";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Date";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 80;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Patient Name";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 300;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Contact No";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // frmPationReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 531);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvPationReport);
            this.Name = "frmPationReport";
            this.Text = "Patient History Report";
            this.Load += new System.EventHandler(this.frmPationReport_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPationReport)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DataGridView dgvPationReport;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.TextBox txtrecept;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}
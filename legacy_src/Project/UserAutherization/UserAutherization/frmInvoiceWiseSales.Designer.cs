namespace UserAutherization
{
    partial class frmInvoiceWiseSales
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkInvTo = new System.Windows.Forms.CheckBox();
            this.chkInvFrom = new System.Windows.Forms.CheckBox();
            this.cmbInvTo = new System.Windows.Forms.ComboBox();
            this.cmbInvFrom = new System.Windows.Forms.ComboBox();
            this.dtpDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpDateFrom = new System.Windows.Forms.DateTimePicker();
            this.chkDateTo = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkDateFrom = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkInvTo);
            this.groupBox1.Controls.Add(this.chkInvFrom);
            this.groupBox1.Controls.Add(this.cmbInvTo);
            this.groupBox1.Controls.Add(this.cmbInvFrom);
            this.groupBox1.Controls.Add(this.dtpDateTo);
            this.groupBox1.Controls.Add(this.dtpDateFrom);
            this.groupBox1.Controls.Add(this.chkDateTo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkDateFrom);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(16, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 167);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transactions";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // chkInvTo
            // 
            this.chkInvTo.AutoSize = true;
            this.chkInvTo.Location = new System.Drawing.Point(124, 127);
            this.chkInvTo.Name = "chkInvTo";
            this.chkInvTo.Size = new System.Drawing.Size(15, 14);
            this.chkInvTo.TabIndex = 5;
            this.chkInvTo.UseVisualStyleBackColor = true;
            this.chkInvTo.CheckedChanged += new System.EventHandler(this.chkInvTo_CheckedChanged);
            // 
            // chkInvFrom
            // 
            this.chkInvFrom.AutoSize = true;
            this.chkInvFrom.Location = new System.Drawing.Point(124, 89);
            this.chkInvFrom.Name = "chkInvFrom";
            this.chkInvFrom.Size = new System.Drawing.Size(15, 14);
            this.chkInvFrom.TabIndex = 4;
            this.chkInvFrom.UseVisualStyleBackColor = true;
            this.chkInvFrom.CheckedChanged += new System.EventHandler(this.chkInvFrom_CheckedChanged);
            // 
            // cmbInvTo
            // 
            this.cmbInvTo.FormattingEnabled = true;
            this.cmbInvTo.Location = new System.Drawing.Point(156, 124);
            this.cmbInvTo.Name = "cmbInvTo";
            this.cmbInvTo.Size = new System.Drawing.Size(122, 21);
            this.cmbInvTo.TabIndex = 3;
            // 
            // cmbInvFrom
            // 
            this.cmbInvFrom.FormattingEnabled = true;
            this.cmbInvFrom.Location = new System.Drawing.Point(156, 86);
            this.cmbInvFrom.Name = "cmbInvFrom";
            this.cmbInvFrom.Size = new System.Drawing.Size(122, 21);
            this.cmbInvFrom.TabIndex = 3;
            // 
            // dtpDateTo
            // 
            this.dtpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateTo.Location = new System.Drawing.Point(156, 56);
            this.dtpDateTo.Name = "dtpDateTo";
            this.dtpDateTo.Size = new System.Drawing.Size(122, 20);
            this.dtpDateTo.TabIndex = 2;
            this.dtpDateTo.Visible = false;
            // 
            // dtpDateFrom
            // 
            this.dtpDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateFrom.Location = new System.Drawing.Point(156, 20);
            this.dtpDateFrom.Name = "dtpDateFrom";
            this.dtpDateFrom.Size = new System.Drawing.Size(122, 20);
            this.dtpDateFrom.TabIndex = 2;
            this.dtpDateFrom.Visible = false;
            // 
            // chkDateTo
            // 
            this.chkDateTo.AutoSize = true;
            this.chkDateTo.Location = new System.Drawing.Point(124, 59);
            this.chkDateTo.Name = "chkDateTo";
            this.chkDateTo.Size = new System.Drawing.Size(15, 14);
            this.chkDateTo.TabIndex = 1;
            this.chkDateTo.UseVisualStyleBackColor = true;
            this.chkDateTo.Visible = false;
            this.chkDateTo.CheckedChanged += new System.EventHandler(this.chkDateTo_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Invoice To";
            // 
            // chkDateFrom
            // 
            this.chkDateFrom.AutoSize = true;
            this.chkDateFrom.Location = new System.Drawing.Point(124, 23);
            this.chkDateFrom.Name = "chkDateFrom";
            this.chkDateFrom.Size = new System.Drawing.Size(15, 14);
            this.chkDateFrom.TabIndex = 1;
            this.chkDateFrom.UseVisualStyleBackColor = true;
            this.chkDateFrom.Visible = false;
            this.chkDateFrom.CheckedChanged += new System.EventHandler(this.chkDateFrom_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Invoice From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Date To";
            this.label2.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Date From";
            this.label1.Visible = false;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(273, 207);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // frmInvoiceWiseSales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(371, 242);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmInvoiceWiseSales";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Invoice Wise Sales";
            this.Load += new System.EventHandler(this.frmInvoiceWiseSales_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpDateTo;
        private System.Windows.Forms.DateTimePicker dtpDateFrom;
        private System.Windows.Forms.CheckBox chkDateTo;
        private System.Windows.Forms.CheckBox chkDateFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.ComboBox cmbInvTo;
        private System.Windows.Forms.ComboBox cmbInvFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkInvTo;
        private System.Windows.Forms.CheckBox chkInvFrom;
    }
}
namespace UserAutherization
{
    partial class frmBranchToHeadOfficecs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBranchToHeadOfficecs));
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkReceipts = new System.Windows.Forms.CheckBox();
            this.chkInvoices = new System.Windows.Forms.CheckBox();
            this.chkCreditNote = new System.Windows.Forms.CheckBox();
            this.chkCustomerMasters = new System.Windows.Forms.CheckBox();
            this.chkTransfers = new System.Windows.Forms.CheckBox();
            this.chkInvAdjustments = new System.Windows.Forms.CheckBox();
            this.btnCreateFiles = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.txtselect = new System.Windows.Forms.TextBox();
            this.gbCreateXML = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox7.SuspendLayout();
            this.gbCreateXML.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox7.Controls.Add(this.chkReceipts);
            this.groupBox7.Controls.Add(this.chkInvoices);
            this.groupBox7.Controls.Add(this.chkCreditNote);
            this.groupBox7.Controls.Add(this.chkCustomerMasters);
            this.groupBox7.Controls.Add(this.chkTransfers);
            this.groupBox7.Controls.Add(this.chkInvAdjustments);
            this.groupBox7.Location = new System.Drawing.Point(11, 14);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(519, 120);
            this.groupBox7.TabIndex = 18;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Import Files";
            // 
            // chkReceipts
            // 
            this.chkReceipts.AutoSize = true;
            this.chkReceipts.Checked = true;
            this.chkReceipts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReceipts.Location = new System.Drawing.Point(264, 56);
            this.chkReceipts.Name = "chkReceipts";
            this.chkReceipts.Size = new System.Drawing.Size(74, 17);
            this.chkReceipts.TabIndex = 5;
            this.chkReceipts.Text = "Receipts";
            this.chkReceipts.UseVisualStyleBackColor = true;
            // 
            // chkInvoices
            // 
            this.chkInvoices.AutoSize = true;
            this.chkInvoices.Checked = true;
            this.chkInvoices.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInvoices.Location = new System.Drawing.Point(264, 27);
            this.chkInvoices.Name = "chkInvoices";
            this.chkInvoices.Size = new System.Drawing.Size(74, 17);
            this.chkInvoices.TabIndex = 4;
            this.chkInvoices.Text = "Invoices";
            this.chkInvoices.UseVisualStyleBackColor = true;
            // 
            // chkCreditNote
            // 
            this.chkCreditNote.AutoSize = true;
            this.chkCreditNote.Checked = true;
            this.chkCreditNote.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreditNote.Location = new System.Drawing.Point(17, 85);
            this.chkCreditNote.Name = "chkCreditNote";
            this.chkCreditNote.Size = new System.Drawing.Size(97, 17);
            this.chkCreditNote.TabIndex = 3;
            this.chkCreditNote.Text = "Credit Notes";
            this.chkCreditNote.UseVisualStyleBackColor = true;
            // 
            // chkCustomerMasters
            // 
            this.chkCustomerMasters.AutoSize = true;
            this.chkCustomerMasters.Checked = true;
            this.chkCustomerMasters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCustomerMasters.Location = new System.Drawing.Point(17, 56);
            this.chkCustomerMasters.Name = "chkCustomerMasters";
            this.chkCustomerMasters.Size = new System.Drawing.Size(124, 17);
            this.chkCustomerMasters.TabIndex = 2;
            this.chkCustomerMasters.Text = "Customer Master";
            this.chkCustomerMasters.UseVisualStyleBackColor = true;
            // 
            // chkTransfers
            // 
            this.chkTransfers.AutoSize = true;
            this.chkTransfers.Location = new System.Drawing.Point(264, 79);
            this.chkTransfers.Name = "chkTransfers";
            this.chkTransfers.Size = new System.Drawing.Size(110, 17);
            this.chkTransfers.TabIndex = 1;
            this.chkTransfers.Text = "Transfer Notes";
            this.chkTransfers.UseVisualStyleBackColor = true;
            this.chkTransfers.Visible = false;
            this.chkTransfers.CheckedChanged += new System.EventHandler(this.chkTransfers_CheckedChanged);
            // 
            // chkInvAdjustments
            // 
            this.chkInvAdjustments.AutoSize = true;
            this.chkInvAdjustments.Checked = true;
            this.chkInvAdjustments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInvAdjustments.Location = new System.Drawing.Point(16, 27);
            this.chkInvAdjustments.Name = "chkInvAdjustments";
            this.chkInvAdjustments.Size = new System.Drawing.Size(157, 17);
            this.chkInvAdjustments.TabIndex = 0;
            this.chkInvAdjustments.Text = "Inventory Adjustments";
            this.chkInvAdjustments.UseVisualStyleBackColor = true;
            // 
            // btnCreateFiles
            // 
            this.btnCreateFiles.Location = new System.Drawing.Point(352, 177);
            this.btnCreateFiles.Name = "btnCreateFiles";
            this.btnCreateFiles.Size = new System.Drawing.Size(93, 37);
            this.btnCreateFiles.TabIndex = 19;
            this.btnCreateFiles.Text = "Create Files";
            this.btnCreateFiles.UseVisualStyleBackColor = true;
            this.btnCreateFiles.Click += new System.EventHandler(this.btnCreateFiles_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Path";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Rockwell", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 303);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 19);
            this.label1.TabIndex = 22;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(496, 147);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(35, 25);
            this.button2.TabIndex = 23;
            this.button2.Text = "......";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtselect
            // 
            this.txtselect.Location = new System.Drawing.Point(66, 151);
            this.txtselect.Name = "txtselect";
            this.txtselect.Size = new System.Drawing.Size(426, 21);
            this.txtselect.TabIndex = 24;
            // 
            // gbCreateXML
            // 
            this.gbCreateXML.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gbCreateXML.Controls.Add(this.button3);
            this.gbCreateXML.Controls.Add(this.txtselect);
            this.gbCreateXML.Controls.Add(this.button2);
            this.gbCreateXML.Controls.Add(this.label1);
            this.gbCreateXML.Controls.Add(this.label3);
            this.gbCreateXML.Controls.Add(this.btnCreateFiles);
            this.gbCreateXML.Controls.Add(this.groupBox7);
            this.gbCreateXML.ForeColor = System.Drawing.Color.Black;
            this.gbCreateXML.Location = new System.Drawing.Point(14, 84);
            this.gbCreateXML.Name = "gbCreateXML";
            this.gbCreateXML.Size = new System.Drawing.Size(545, 223);
            this.gbCreateXML.TabIndex = 14;
            this.gbCreateXML.TabStop = false;
            this.gbCreateXML.Text = " ";
            this.gbCreateXML.UseCompatibleTextRendering = true;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Image = global::UserAutherization.Properties.Resources.close1;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(450, 177);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 37);
            this.button3.TabIndex = 25;
            this.button3.Text = "Exit";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // OFD
            // 
            this.OFD.FileName = "openFileDialog1";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            this.printPreviewDialog1.Load += new System.EventHandler(this.printPreviewDialog1_Load);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpFromDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpToDate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(14, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(537, 66);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Date Range";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(52, 21);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(144, 21);
            this.dtpFromDate.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "From";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(264, 21);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(153, 21);
            this.dtpToDate.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(228, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "To";
            // 
            // frmBranchToHeadOfficecs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(569, 313);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbCreateXML);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmBranchToHeadOfficecs";
            this.Text = "Import Export Branch to Head Office";
            this.Load += new System.EventHandler(this.frmBranchToHeadOfficecs_Load);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.gbCreateXML.ResumeLayout(false);
            this.gbCreateXML.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox chkReceipts;
        private System.Windows.Forms.CheckBox chkInvoices;
        private System.Windows.Forms.CheckBox chkCreditNote;
        private System.Windows.Forms.CheckBox chkCustomerMasters;
        private System.Windows.Forms.CheckBox chkTransfers;
        private System.Windows.Forms.CheckBox chkInvAdjustments;
        private System.Windows.Forms.Button btnCreateFiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtselect;
        private System.Windows.Forms.GroupBox gbCreateXML;
        public System.Windows.Forms.Button button3;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label label4;

    }
}
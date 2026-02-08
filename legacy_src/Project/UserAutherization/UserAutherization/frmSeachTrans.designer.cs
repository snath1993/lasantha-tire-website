namespace UserAutherization

{
    partial class frmSeachTrans
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSeachTrans));
            this.dgvSearchIssue = new System.Windows.Forms.DataGridView();
            this.InvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IssueNoteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToWarehouse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpSearchDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchIssue)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSearchIssue
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSearchIssue.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearchIssue.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvSearchIssue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchIssue.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSearchIssue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchIssue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.InvoiceNo,
            this.IssueNoteID,
            this.ToWarehouse,
            this.date});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchIssue.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSearchIssue.EnableHeadersVisualStyles = false;
            this.dgvSearchIssue.GridColor = System.Drawing.Color.Silver;
            this.dgvSearchIssue.Location = new System.Drawing.Point(14, 83);
            this.dgvSearchIssue.Name = "dgvSearchIssue";
            this.dgvSearchIssue.RowHeadersVisible = false;
            this.dgvSearchIssue.Size = new System.Drawing.Size(784, 520);
            this.dgvSearchIssue.TabIndex = 57;
            this.dgvSearchIssue.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchIssue_CellDoubleClick_1);
            this.dgvSearchIssue.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchIssue_CellContentClick);
            // 
            // InvoiceNo
            // 
            this.InvoiceNo.HeaderText = "Tranfer No";
            this.InvoiceNo.Name = "InvoiceNo";
            this.InvoiceNo.ReadOnly = true;
            this.InvoiceNo.Width = 160;
            // 
            // IssueNoteID
            // 
            this.IssueNoteID.HeaderText = "From Warehouse";
            this.IssueNoteID.Name = "IssueNoteID";
            this.IssueNoteID.ReadOnly = true;
            this.IssueNoteID.Width = 145;
            // 
            // ToWarehouse
            // 
            this.ToWarehouse.HeaderText = "To Warehouse";
            this.ToWarehouse.Name = "ToWarehouse";
            this.ToWarehouse.Width = 130;
            // 
            // date
            // 
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 160;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnClear.Location = new System.Drawing.Point(639, 42);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(98, 27);
            this.btnClear.TabIndex = 70;
            this.btnClear.Text = "Clear Search";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.BackColor = System.Drawing.Color.Transparent;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.Location = new System.Drawing.Point(459, 46);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(31, 15);
            this.lblDate.TabIndex = 68;
            this.lblDate.Text = "&Date";
            // 
            // dtpSearchDate
            // 
            this.dtpSearchDate.CalendarFont = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpSearchDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpSearchDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSearchDate.Location = new System.Drawing.Point(502, 42);
            this.dtpSearchDate.Name = "dtpSearchDate";
            this.dtpSearchDate.Size = new System.Drawing.Size(122, 23);
            this.dtpSearchDate.TabIndex = 69;
            this.dtpSearchDate.ValueChanged += new System.EventHandler(this.dtpSearchDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 15);
            this.label6.TabIndex = 66;
            this.label6.Text = "Search By";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(250, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 15);
            this.label5.TabIndex = 67;
            this.label5.Text = "Search";
            // 
            // cmbSearchby
            // 
            this.cmbSearchby.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchby.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSearchby.FormattingEnabled = true;
            this.cmbSearchby.Items.AddRange(new object[] {
            "Transfer No",
            "From Warehouse",
            "To Warehouse",
            "Date"});
            this.cmbSearchby.Location = new System.Drawing.Point(82, 42);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(160, 23);
            this.cmbSearchby.TabIndex = 64;
            this.cmbSearchby.SelectedIndexChanged += new System.EventHandler(this.cmbSearchby_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(306, 42);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(145, 23);
            this.txtSearch.TabIndex = 65;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(811, 25);
            this.toolStrip1.TabIndex = 71;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(56, 22);
            this.toolStripButton1.Text = "Close";
            this.toolStripButton1.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // frmSeachTrans
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(811, 617);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dgvSearchIssue);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpSearchDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.txtSearch);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(145, 155);
            this.Name = "frmSeachTrans";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer Note  List";
            this.Load += new System.EventHandler(this.frmSeachTrans_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchIssue)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSearchIssue;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpSearchDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSearchby;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn IssueNoteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ToWarehouse;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}
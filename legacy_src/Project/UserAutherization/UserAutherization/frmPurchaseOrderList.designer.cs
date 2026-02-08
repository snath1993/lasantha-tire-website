namespace UserAutherization
{
    partial class frmPurchaseOrderList

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPurchaseOrderList));
            this.btnclose = new System.Windows.Forms.Button();
            this.dgvSearchIssue = new System.Windows.Forms.DataGridView();
            this.CRNNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IssueNoteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WarehouseID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpSearchDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchIssue)).BeginInit();
            this.SuspendLayout();
            // 
            // btnclose
            // 
            this.btnclose.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnclose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnclose.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclose.Image = global::UserAutherization.Properties.Resources.Close;
            this.btnclose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnclose.Location = new System.Drawing.Point(12, 10);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(45, 35);
            this.btnclose.TabIndex = 56;
            this.btnclose.Text = "Close";
            this.btnclose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnclose.UseVisualStyleBackColor = false;
            this.btnclose.Visible = false;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // dgvSearchIssue
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSearchIssue.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearchIssue.BackgroundColor = System.Drawing.Color.White;
            this.dgvSearchIssue.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvSearchIssue.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchIssue.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSearchIssue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchIssue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CRNNo,
            this.IssueNoteID,
            this.date,
            this.WarehouseID,
            this.Amount,
            this.Status});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchIssue.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSearchIssue.Location = new System.Drawing.Point(7, 72);
            this.dgvSearchIssue.Name = "dgvSearchIssue";
            this.dgvSearchIssue.RowHeadersVisible = false;
            this.dgvSearchIssue.Size = new System.Drawing.Size(847, 451);
            this.dgvSearchIssue.TabIndex = 57;
            this.dgvSearchIssue.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchIssue_CellContentClick);
            this.dgvSearchIssue.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchIssue_CellDoubleClick);
            this.dgvSearchIssue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvSearchIssue_KeyDown);
            // 
            // CRNNo
            // 
            this.CRNNo.HeaderText = "Purchase No";
            this.CRNNo.Name = "CRNNo";
            this.CRNNo.ReadOnly = true;
            this.CRNNo.Width = 130;
            // 
            // IssueNoteID
            // 
            this.IssueNoteID.HeaderText = "Vendor ID";
            this.IssueNoteID.Name = "IssueNoteID";
            this.IssueNoteID.ReadOnly = true;
            this.IssueNoteID.Width = 145;
            // 
            // date
            // 
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 160;
            // 
            // WarehouseID
            // 
            this.WarehouseID.HeaderText = "Warehouse ID";
            this.WarehouseID.Name = "WarehouseID";
            this.WarehouseID.ReadOnly = true;
            this.WarehouseID.Width = 160;
            // 
            // Amount
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Amount.DefaultCellStyle = dataGridViewCellStyle3;
            this.Amount.HeaderText = "PO Total";
            this.Amount.Name = "Amount";
            this.Amount.Width = 150;
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnClear.Location = new System.Drawing.Point(600, 23);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(84, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear Search";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.Location = new System.Drawing.Point(446, 27);
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
            this.dtpSearchDate.Location = new System.Drawing.Point(483, 24);
            this.dtpSearchDate.Name = "dtpSearchDate";
            this.dtpSearchDate.Size = new System.Drawing.Size(105, 23);
            this.dtpSearchDate.TabIndex = 3;
            this.dtpSearchDate.ValueChanged += new System.EventHandler(this.dtpSearchDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(63, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 15);
            this.label6.TabIndex = 66;
            this.label6.Text = "Search By";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(267, 27);
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
            "PONumber",
            "VendorID",
            "Date",
            "WHID",
            "Save",
            "Process"});
            this.cmbSearchby.Location = new System.Drawing.Point(123, 24);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(138, 23);
            this.cmbSearchby.TabIndex = 1;
            this.cmbSearchby.SelectedIndexChanged += new System.EventHandler(this.cmbSearchby_SelectedIndexChanged);
            this.cmbSearchby.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbSearchby_KeyDown);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(315, 24);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(125, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // frmPurchaseOrderList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(866, 535);
            this.Controls.Add(this.dgvSearchIssue);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpSearchDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnclose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(145, 155);
            this.Name = "frmPurchaseOrderList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase Order List";
            this.Load += new System.EventHandler(this.frmInvoiceSearch_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPurchaseOrderList_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchIssue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.DataGridView dgvSearchIssue;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpSearchDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSearchby;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridViewTextBoxColumn CRNNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn IssueNoteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn WarehouseID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}
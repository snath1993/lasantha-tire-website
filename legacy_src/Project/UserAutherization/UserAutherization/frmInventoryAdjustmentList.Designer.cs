namespace UserAutherization
{
    partial class frmInventoryAdjustmentList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInventoryAdjustmentList));
            this.dgvInvoiceList = new System.Windows.Forms.DataGridView();
            this.DDeliveryNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Customer_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoNos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GRNNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpSearchDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoiceList)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvInvoiceList
            // 
            this.dgvInvoiceList.AllowUserToResizeColumns = false;
            this.dgvInvoiceList.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvInvoiceList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvInvoiceList.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvInvoiceList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInvoiceList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvInvoiceList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvoiceList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DDeliveryNo,
            this.Customer_ID,
            this.Date,
            this.SoNos,
            this.GRNNo,
            this.TotalAmount});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvInvoiceList.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvInvoiceList.EnableHeadersVisualStyles = false;
            this.dgvInvoiceList.GridColor = System.Drawing.Color.Silver;
            this.dgvInvoiceList.Location = new System.Drawing.Point(14, 89);
            this.dgvInvoiceList.Name = "dgvInvoiceList";
            this.dgvInvoiceList.RowHeadersVisible = false;
            this.dgvInvoiceList.Size = new System.Drawing.Size(804, 504);
            this.dgvInvoiceList.TabIndex = 55;
            this.dgvInvoiceList.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvInvoiceList_CellContentDoubleClick);
            this.dgvInvoiceList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvInvoiceList_CellDoubleClick);
            // 
            // DDeliveryNo
            // 
            this.DDeliveryNo.HeaderText = "AdjustmentCode";
            this.DDeliveryNo.Name = "DDeliveryNo";
            this.DDeliveryNo.Width = 140;
            // 
            // Customer_ID
            // 
            this.Customer_ID.HeaderText = "Date";
            this.Customer_ID.Name = "Customer_ID";
            this.Customer_ID.Width = 140;
            // 
            // Date
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Date.DefaultCellStyle = dataGridViewCellStyle3;
            this.Date.HeaderText = "WarehouseID";
            this.Date.Name = "Date";
            // 
            // SoNos
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.SoNos.DefaultCellStyle = dataGridViewCellStyle4;
            this.SoNos.HeaderText = "Item ID";
            this.SoNos.Name = "SoNos";
            this.SoNos.Width = 140;
            // 
            // GRNNo
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.GRNNo.DefaultCellStyle = dataGridViewCellStyle5;
            this.GRNNo.HeaderText = "Quantity";
            this.GRNNo.Name = "GRNNo";
            this.GRNNo.Width = 140;
            // 
            // TotalAmount
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TotalAmount.DefaultCellStyle = dataGridViewCellStyle6;
            this.TotalAmount.HeaderText = "Unit Cost";
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.Width = 140;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.BackColor = System.Drawing.Color.Transparent;
            this.lblDate.Location = new System.Drawing.Point(531, 46);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(31, 15);
            this.lblDate.TabIndex = 62;
            this.lblDate.Text = "&Date";
            // 
            // dtpSearchDate
            // 
            this.dtpSearchDate.CalendarFont = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpSearchDate.CustomFormat = "dd/MM/yyyy";
            this.dtpSearchDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSearchDate.Location = new System.Drawing.Point(573, 42);
            this.dtpSearchDate.Name = "dtpSearchDate";
            this.dtpSearchDate.Size = new System.Drawing.Size(122, 23);
            this.dtpSearchDate.TabIndex = 63;
            this.dtpSearchDate.ValueChanged += new System.EventHandler(this.dtpSearchDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(14, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 15);
            this.label6.TabIndex = 59;
            this.label6.Text = "Search By";
            // 
            // cmbSearchby
            // 
            this.cmbSearchby.FormattingEnabled = true;
            this.cmbSearchby.Items.AddRange(new object[] {
            "AdjustmentCode",
            "Date",
            "WarehouseCode",
            "Item ID"});
            this.cmbSearchby.Location = new System.Drawing.Point(77, 42);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(148, 23);
            this.cmbSearchby.TabIndex = 57;
            this.cmbSearchby.Text = "AdjustmentCode";
            this.cmbSearchby.SelectedIndexChanged += new System.EventHandler(this.cmbSearchby_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Location = new System.Drawing.Point(229, 42);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(294, 23);
            this.txtSearch.TabIndex = 58;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(960, 25);
            this.toolStrip1.TabIndex = 64;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(56, 22);
            this.toolStripButton1.Text = "Close";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // ultraButton1
            // 
            this.ultraButton1.Location = new System.Drawing.Point(722, 40);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(96, 27);
            this.ultraButton1.TabIndex = 65;
            this.ultraButton1.Text = "Clear Search";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(887, 69);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 98);
            this.panel1.TabIndex = 66;
            this.panel1.Visible = false;
            // 
            // frmInventoryAdjustmentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(960, 607);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ultraButton1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpSearchDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.dgvInvoiceList);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmInventoryAdjustmentList";
            this.Text = "Inventory Adjustment List";
            this.Load += new System.EventHandler(this.frmInventoryAdjustmentList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoiceList)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvInvoiceList;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpSearchDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbSearchby;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private System.Windows.Forms.DataGridViewTextBoxColumn DDeliveryNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Customer_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoNos;
        private System.Windows.Forms.DataGridViewTextBoxColumn GRNNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
        private System.Windows.Forms.Panel panel1;
    }
}
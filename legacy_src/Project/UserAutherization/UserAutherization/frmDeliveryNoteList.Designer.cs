namespace UserAutherization
{
    partial class frmDeliveryNoteList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDeliveryNoteList));
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvDeliveryNoteList = new System.Windows.Forms.DataGridView();
            this.DDeliveryNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Customer_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoNos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WareHose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpSearchDate = new System.Windows.Forms.DateTimePicker();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeliveryNoteList)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(14, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 42;
            this.label6.Text = "Search By";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(251, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "Search";
            // 
            // cmbSearchby
            // 
            this.cmbSearchby.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchby.FormattingEnabled = true;
            this.cmbSearchby.Items.AddRange(new object[] {
            "GRN NO",
            "VendorID",
            "Date",
            "PO Number",
            "Location",
            "Save",
            "Process"});
            this.cmbSearchby.Location = new System.Drawing.Point(98, 38);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(148, 21);
            this.cmbSearchby.TabIndex = 40;
            this.cmbSearchby.SelectedIndexChanged += new System.EventHandler(this.cmbSearchby_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(304, 37);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(161, 22);
            this.txtSearch.TabIndex = 41;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // dgvDeliveryNoteList
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvDeliveryNoteList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDeliveryNoteList.BackgroundColor = System.Drawing.Color.White;
            this.dgvDeliveryNoteList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDeliveryNoteList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDeliveryNoteList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDeliveryNoteList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DDeliveryNo,
            this.Customer_ID,
            this.Date,
            this.TotalAmount,
            this.SoNos,
            this.WareHose,
            this.Status});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDeliveryNoteList.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDeliveryNoteList.EnableHeadersVisualStyles = false;
            this.dgvDeliveryNoteList.Location = new System.Drawing.Point(8, 75);
            this.dgvDeliveryNoteList.Name = "dgvDeliveryNoteList";
            this.dgvDeliveryNoteList.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.dgvDeliveryNoteList.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvDeliveryNoteList.Size = new System.Drawing.Size(941, 448);
            this.dgvDeliveryNoteList.TabIndex = 44;
            this.dgvDeliveryNoteList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDeliveryNoteList_CellDoubleClick);
            // 
            // DDeliveryNo
            // 
            this.DDeliveryNo.HeaderText = "GRN No";
            this.DDeliveryNo.Name = "DDeliveryNo";
            this.DDeliveryNo.Width = 170;
            // 
            // Customer_ID
            // 
            this.Customer_ID.HeaderText = "Vendor ID";
            this.Customer_ID.Name = "Customer_ID";
            this.Customer_ID.Width = 160;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.Width = 170;
            // 
            // TotalAmount
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TotalAmount.DefaultCellStyle = dataGridViewCellStyle3;
            this.TotalAmount.HeaderText = "Amount";
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.Visible = false;
            this.TotalAmount.Width = 140;
            // 
            // SoNos
            // 
            this.SoNos.HeaderText = "PO Number";
            this.SoNos.Name = "SoNos";
            this.SoNos.Width = 170;
            // 
            // WareHose
            // 
            this.WareHose.HeaderText = "Location";
            this.WareHose.Name = "WareHose";
            this.WareHose.Width = 170;
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
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnClear.Location = new System.Drawing.Point(658, 34);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(105, 23);
            this.btnClear.TabIndex = 46;
            this.btnClear.Text = "Clear Search";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.BackColor = System.Drawing.Color.Transparent;
            this.lblDate.Location = new System.Drawing.Point(478, 39);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(34, 13);
            this.lblDate.TabIndex = 47;
            this.lblDate.Text = "&Date";
            // 
            // dtpSearchDate
            // 
            this.dtpSearchDate.CalendarFont = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpSearchDate.CustomFormat = "dd/MM/yyyy";
            this.dtpSearchDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSearchDate.Location = new System.Drawing.Point(525, 35);
            this.dtpSearchDate.Name = "dtpSearchDate";
            this.dtpSearchDate.Size = new System.Drawing.Size(105, 21);
            this.dtpSearchDate.TabIndex = 48;
            this.dtpSearchDate.ValueChanged += new System.EventHandler(this.dtpSearchDate_ValueChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(960, 25);
            this.toolStrip1.TabIndex = 66;
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
            // frmDeliveryNoteList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(960, 535);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpSearchDate);
            this.Controls.Add(this.dgvDeliveryNoteList);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.txtSearch);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(145, 155);
            this.Name = "frmDeliveryNoteList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Goods Received Note List";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDeliveryNoteList_FormClosed);
            this.Load += new System.EventHandler(this.frmDeliveryNoteList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeliveryNoteList)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSearchby;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgvDeliveryNoteList;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpSearchDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn DDeliveryNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Customer_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoNos;
        private System.Windows.Forms.DataGridViewTextBoxColumn WareHose;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}
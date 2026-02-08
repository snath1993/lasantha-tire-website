namespace UserAutherization
{
    partial class ScanList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanList));
            this.btnClear = new System.Windows.Forms.Button();
            this.dgvSearchEmployee = new System.Windows.Forms.DataGridView();
            this.btnSearchByDate = new System.Windows.Forms.Button();
            this.dtpSearchDate = new System.Windows.Forms.DateTimePicker();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.ReceiptNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Consultant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsConfirm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsVoid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchEmployee)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnClear.Location = new System.Drawing.Point(889, 10);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(117, 23);
            this.btnClear.TabIndex = 276;
            this.btnClear.Text = "Clear Search";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dgvSearchEmployee
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSearchEmployee.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearchEmployee.BackgroundColor = System.Drawing.Color.White;
            this.dgvSearchEmployee.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchEmployee.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSearchEmployee.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchEmployee.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ReceiptNo,
            this.Column1,
            this.PatientName,
            this.Consultant,
            this.ItemType,
            this.Date,
            this.IsConfirm,
            this.IsVoid});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchEmployee.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSearchEmployee.Location = new System.Drawing.Point(2, 46);
            this.dgvSearchEmployee.Name = "dgvSearchEmployee";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchEmployee.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSearchEmployee.RowHeadersVisible = false;
            this.dgvSearchEmployee.Size = new System.Drawing.Size(1038, 383);
            this.dgvSearchEmployee.TabIndex = 275;
            this.dgvSearchEmployee.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchEmployee_CellDoubleClick);
            // 
            // btnSearchByDate
            // 
            this.btnSearchByDate.Enabled = false;
            this.btnSearchByDate.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchByDate.Location = new System.Drawing.Point(677, 10);
            this.btnSearchByDate.Name = "btnSearchByDate";
            this.btnSearchByDate.Size = new System.Drawing.Size(197, 23);
            this.btnSearchByDate.TabIndex = 274;
            this.btnSearchByDate.Text = "Search by Date";
            this.btnSearchByDate.UseVisualStyleBackColor = true;
            this.btnSearchByDate.Click += new System.EventHandler(this.btnSearchByDate_Click);
            // 
            // dtpSearchDate
            // 
            this.dtpSearchDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSearchDate.Location = new System.Drawing.Point(461, 11);
            this.dtpSearchDate.Name = "dtpSearchDate";
            this.dtpSearchDate.Size = new System.Drawing.Size(149, 20);
            this.dtpSearchDate.TabIndex = 273;
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(306, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(149, 22);
            this.txtSearch.TabIndex = 272;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(244, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 271;
            this.label1.Text = "Search  :";
            // 
            // cmbSearchby
            // 
            this.cmbSearchby.BackColor = System.Drawing.Color.White;
            this.cmbSearchby.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchby.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSearchby.ForeColor = System.Drawing.Color.Black;
            this.cmbSearchby.FormattingEnabled = true;
            this.cmbSearchby.Items.AddRange(new object[] {
            "Receipt No",
            "Patient Name",
            "Consultant",
            "ItemType",
            "Date"});
            this.cmbSearchby.Location = new System.Drawing.Point(85, 10);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(149, 22);
            this.cmbSearchby.TabIndex = 270;
            this.cmbSearchby.SelectedIndexChanged += new System.EventHandler(this.cmbSearchby_SelectedIndexChanged);
            this.cmbSearchby.Click += new System.EventHandler(this.cmbSearchby_Click);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(2, 14);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(82, 14);
            this.label31.TabIndex = 269;
            this.label31.Text = "Search By  :";
            // 
            // ReceiptNo
            // 
            this.ReceiptNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ReceiptNo.HeaderText = "Receipt No";
            this.ReceiptNo.Name = "ReceiptNo";
            this.ReceiptNo.Width = 105;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "No";
            this.Column1.Name = "Column1";
            this.Column1.Width = 50;
            // 
            // PatientName
            // 
            this.PatientName.HeaderText = "Patient Name";
            this.PatientName.Name = "PatientName";
            this.PatientName.Width = 250;
            // 
            // Consultant
            // 
            this.Consultant.HeaderText = "Consultant";
            this.Consultant.Name = "Consultant";
            this.Consultant.Width = 250;
            // 
            // ItemType
            // 
            this.ItemType.HeaderText = "Item Type";
            this.ItemType.Name = "ItemType";
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.Width = 120;
            // 
            // IsConfirm
            // 
            this.IsConfirm.HeaderText = "Confirm";
            this.IsConfirm.Name = "IsConfirm";
            this.IsConfirm.Width = 75;
            // 
            // IsVoid
            // 
            this.IsVoid.HeaderText = "Void";
            this.IsVoid.Name = "IsVoid";
            this.IsVoid.Width = 75;
            // 
            // ScanList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(1041, 450);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.dgvSearchEmployee);
            this.Controls.Add(this.btnSearchByDate);
            this.Controls.Add(this.dtpSearchDate);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.label31);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScanList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Receipt List";
            this.Load += new System.EventHandler(this.ScanList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchEmployee)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGridView dgvSearchEmployee;
        private System.Windows.Forms.Button btnSearchByDate;
        private System.Windows.Forms.DateTimePicker dtpSearchDate;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSearchby;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReceiptNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Consultant;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsConfirm;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsVoid;
    }
}
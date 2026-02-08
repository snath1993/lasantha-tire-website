namespace UserAutherization
{
    partial class InpatientMasterList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InpatientMasterList));
            this.btnClear = new System.Windows.Forms.Button();
            this.dgvSearchEmployee = new System.Windows.Forms.DataGridView();
            this.PatientNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NicNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Age = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Doctor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSearchByDate = new System.Windows.Forms.Button();
            this.dtpSearchDate = new System.Windows.Forms.DateTimePicker();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchEmployee)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnClear.Location = new System.Drawing.Point(715, 22);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(99, 23);
            this.btnClear.TabIndex = 268;
            this.btnClear.Text = "Clear Search";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dgvSearchEmployee
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvSearchEmployee.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearchEmployee.BackgroundColor = System.Drawing.Color.White;
            this.dgvSearchEmployee.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchEmployee.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSearchEmployee.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchEmployee.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PatientNo,
            this.Type,
            this.Name,
            this.NicNumber,
            this.Age,
            this.ContactNo,
            this.Gender,
            this.Doctor,
            this.Date});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchEmployee.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSearchEmployee.Location = new System.Drawing.Point(4, 55);
            this.dgvSearchEmployee.Name = "dgvSearchEmployee";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchEmployee.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSearchEmployee.RowHeadersVisible = false;
            this.dgvSearchEmployee.Size = new System.Drawing.Size(810, 386);
            this.dgvSearchEmployee.TabIndex = 267;
            this.dgvSearchEmployee.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchEmployee_CellContentClick);
            this.dgvSearchEmployee.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchEmployee_CellDoubleClick);
            // 
            // PatientNo
            // 
            this.PatientNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PatientNo.HeaderText = "Patient No";
            this.PatientNo.Name = "PatientNo";
            this.PatientNo.Width = 110;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            // 
            // Name
            // 
            this.Name.HeaderText = "Name";
            this.Name.Name = "Name";
            this.Name.Width = 250;
            // 
            // NicNumber
            // 
            this.NicNumber.HeaderText = "NIC ";
            this.NicNumber.Name = "NicNumber";
            this.NicNumber.Width = 120;
            // 
            // Age
            // 
            this.Age.HeaderText = "Age";
            this.Age.Name = "Age";
            // 
            // ContactNo
            // 
            this.ContactNo.HeaderText = "Contact No";
            this.ContactNo.Name = "ContactNo";
            this.ContactNo.Width = 120;
            // 
            // Gender
            // 
            this.Gender.HeaderText = "Gender";
            this.Gender.Name = "Gender";
            this.Gender.Visible = false;
            // 
            // Doctor
            // 
            this.Doctor.HeaderText = "Doctor";
            this.Doctor.Name = "Doctor";
            this.Doctor.Visible = false;
            this.Doctor.Width = 150;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.Visible = false;
            this.Date.Width = 120;
            // 
            // btnSearchByDate
            // 
            this.btnSearchByDate.Enabled = false;
            this.btnSearchByDate.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchByDate.Location = new System.Drawing.Point(617, 23);
            this.btnSearchByDate.Name = "btnSearchByDate";
            this.btnSearchByDate.Size = new System.Drawing.Size(92, 23);
            this.btnSearchByDate.TabIndex = 266;
            this.btnSearchByDate.Text = "Date";
            this.btnSearchByDate.UseVisualStyleBackColor = true;
            this.btnSearchByDate.Click += new System.EventHandler(this.btnSearchByDate_Click);
            // 
            // dtpSearchDate
            // 
            this.dtpSearchDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSearchDate.Location = new System.Drawing.Point(462, 25);
            this.dtpSearchDate.Name = "dtpSearchDate";
            this.dtpSearchDate.Size = new System.Drawing.Size(149, 20);
            this.dtpSearchDate.TabIndex = 265;
            this.dtpSearchDate.ValueChanged += new System.EventHandler(this.dtpSearchDate_ValueChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(307, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(149, 23);
            this.txtSearch.TabIndex = 264;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(242, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 263;
            this.label1.Text = "Search  :";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // cmbSearchby
            // 
            this.cmbSearchby.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchby.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSearchby.FormattingEnabled = true;
            this.cmbSearchby.Items.AddRange(new object[] {
            "Patient No",
            "Type",
            "Name",
            "NIC",
            "Age",
            "Gender",
            "Contact No",
            "Date"});
            this.cmbSearchby.Location = new System.Drawing.Point(87, 22);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(149, 24);
            this.cmbSearchby.TabIndex = 262;
            this.cmbSearchby.SelectedIndexChanged += new System.EventHandler(this.cmbSearchby_SelectedIndexChanged);
            this.cmbSearchby.Click += new System.EventHandler(this.cmbSearchby_Click);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(6, 21);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(91, 16);
            this.label31.TabIndex = 261;
            this.label31.Text = "Search By  :";
            this.label31.Click += new System.EventHandler(this.label31_Click);
            // 
            // InpatientMasterList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(822, 443);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.dgvSearchEmployee);
            this.Controls.Add(this.btnSearchByDate);
            this.Controls.Add(this.dtpSearchDate);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.label31);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          //  this.Name = "InpatientMasterList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PATIENT LIST";
            this.Load += new System.EventHandler(this.InpatientMasterList_Load);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn NicNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Age;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gender;
        private System.Windows.Forms.DataGridViewTextBoxColumn Doctor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
    }
}
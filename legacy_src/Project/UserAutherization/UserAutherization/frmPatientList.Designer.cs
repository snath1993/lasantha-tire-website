namespace UserAutherization
{
    partial class frmPatientList
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
            this.dgvPatientList = new System.Windows.Forms.DataGridView();
            this.btnClear = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnclose = new System.Windows.Forms.Button();
            this.AdmitDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NIC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefDoctor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatientList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPatientList
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvPatientList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPatientList.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvPatientList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPatientList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPatientList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPatientList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AdmitDate,
            this.PatientCode,
            this.PatientName,
            this.ContactNo,
            this.NIC,
            this.RefDoctor,
            this.PType});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPatientList.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvPatientList.Location = new System.Drawing.Point(1, 53);
            this.dgvPatientList.Name = "dgvPatientList";
            this.dgvPatientList.RowHeadersVisible = false;
            this.dgvPatientList.Size = new System.Drawing.Size(800, 451);
            this.dgvPatientList.TabIndex = 58;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnClear.Location = new System.Drawing.Point(621, 18);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(84, 23);
            this.btnClear.TabIndex = 76;
            this.btnClear.Text = "Clear Search";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(86, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 15);
            this.label6.TabIndex = 74;
            this.label6.Text = "Search By";
            // 
            // cmbSearchby
            // 
            this.cmbSearchby.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbSearchby.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSearchby.FormattingEnabled = true;
            this.cmbSearchby.Items.AddRange(new object[] {
            "Patient Code",
            "Patient Name",
            "Contact No",
            "NIC",
            "Ref Doc",
            "Type"});
            this.cmbSearchby.Location = new System.Drawing.Point(161, 19);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(206, 23);
            this.cmbSearchby.TabIndex = 72;
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(393, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(213, 23);
            this.txtSearch.TabIndex = 73;
            // 
            // btnclose
            // 
            this.btnclose.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnclose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnclose.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclose.Image = global::UserAutherization.Properties.Resources.Close;
            this.btnclose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnclose.Location = new System.Drawing.Point(12, 12);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(45, 35);
            this.btnclose.TabIndex = 71;
            this.btnclose.Text = "Close";
            this.btnclose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnclose.UseVisualStyleBackColor = false;
            // 
            // AdmitDate
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.AdmitDate.DefaultCellStyle = dataGridViewCellStyle3;
            this.AdmitDate.HeaderText = "Admit Date";
            this.AdmitDate.Name = "AdmitDate";
            // 
            // PatientCode
            // 
            this.PatientCode.HeaderText = "Patient Code";
            this.PatientCode.Name = "PatientCode";
            this.PatientCode.ReadOnly = true;
            // 
            // PatientName
            // 
            this.PatientName.HeaderText = "Patient Name";
            this.PatientName.Name = "PatientName";
            this.PatientName.ReadOnly = true;
            this.PatientName.Width = 200;
            // 
            // ContactNo
            // 
            this.ContactNo.HeaderText = "Contact No";
            this.ContactNo.Name = "ContactNo";
            this.ContactNo.ReadOnly = true;
            // 
            // NIC
            // 
            this.NIC.HeaderText = "NIC";
            this.NIC.Name = "NIC";
            // 
            // RefDoctor
            // 
            this.RefDoctor.HeaderText = "Ref Doctor";
            this.RefDoctor.Name = "RefDoctor";
            // 
            // PType
            // 
            this.PType.HeaderText = "Type";
            this.PType.Name = "PType";
            this.PType.ReadOnly = true;
            // 
            // frmPatientList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(813, 497);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnclose);
            this.Controls.Add(this.dgvPatientList);
            this.Name = "frmPatientList";
            this.Text = "Patients List";
            this.Load += new System.EventHandler(this.frmPatientList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatientList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPatientList;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdmitDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn NIC;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefDoctor;
        private System.Windows.Forms.DataGridViewTextBoxColumn PType;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbSearchby;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnclose;
    }
}
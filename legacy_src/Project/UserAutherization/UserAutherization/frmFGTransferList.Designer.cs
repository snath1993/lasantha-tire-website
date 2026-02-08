namespace WarehouseTransfer
{
    partial class frmFGTransferList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnclose = new System.Windows.Forms.Button();
            this.dgvSearchIssue = new System.Windows.Forms.DataGridView();
            this.IssueNoteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WarehouseID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Job = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.btnclose.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnclose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnclose.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclose.Image = global::WarehouseTransfer.Properties.Resources.Close;
            this.btnclose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnclose.Location = new System.Drawing.Point(12, 10);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(54, 35);
            this.btnclose.TabIndex = 56;
            this.btnclose.Text = "Close";
            this.btnclose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnclose.UseVisualStyleBackColor = false;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // dgvSearchIssue
            // 
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSearchIssue.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSearchIssue.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchIssue.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSearchIssue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchIssue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IssueNoteID,
            this.date,
            this.WarehouseID,
            this.Job});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchIssue.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSearchIssue.Location = new System.Drawing.Point(12, 72);
            this.dgvSearchIssue.Name = "dgvSearchIssue";
            this.dgvSearchIssue.RowHeadersVisible = false;
            this.dgvSearchIssue.Size = new System.Drawing.Size(672, 489);
            this.dgvSearchIssue.TabIndex = 57;
            this.dgvSearchIssue.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchIssue_CellDoubleClick);
            // 
            // IssueNoteID
            // 
            this.IssueNoteID.HeaderText = "Return Note No";
            this.IssueNoteID.Name = "IssueNoteID";
            this.IssueNoteID.ReadOnly = true;
            this.IssueNoteID.Width = 170;
            // 
            // date
            // 
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.Width = 160;
            // 
            // WarehouseID
            // 
            this.WarehouseID.HeaderText = "Warehouse ID";
            this.WarehouseID.Name = "WarehouseID";
            this.WarehouseID.ReadOnly = true;
            this.WarehouseID.Width = 180;
            // 
            // Job
            // 
            this.Job.HeaderText = "Job";
            this.Job.Name = "Job";
            this.Job.Width = 155;
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
            this.btnClear.TabIndex = 70;
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
            this.dtpSearchDate.Location = new System.Drawing.Point(483, 23);
            this.dtpSearchDate.Name = "dtpSearchDate";
            this.dtpSearchDate.Size = new System.Drawing.Size(105, 23);
            this.dtpSearchDate.TabIndex = 69;
            this.dtpSearchDate.ValueChanged += new System.EventHandler(this.dtpSearchDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(74, 25);
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
            "FG TRans No",
            "Date",
            "Warehouse",
            "Job"});
            this.cmbSearchby.Location = new System.Drawing.Point(136, 22);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(125, 23);
            this.cmbSearchby.TabIndex = 64;
            this.cmbSearchby.SelectedIndexChanged += new System.EventHandler(this.cmbSearchby_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(315, 24);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(125, 23);
            this.txtSearch.TabIndex = 65;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // frmFGTransferList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(695, 573);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpSearchDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.dgvSearchIssue);
            this.Controls.Add(this.btnclose);
            this.Location = new System.Drawing.Point(145, 155);
            this.Name = "frmFGTransferList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FG TRansfer List";
            this.Load += new System.EventHandler(this.frmReturnNoteList_Load);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn IssueNoteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn WarehouseID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Job;
    }
}
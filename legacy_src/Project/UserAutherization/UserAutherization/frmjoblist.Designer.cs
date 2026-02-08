namespace UserAutherization
{
    partial class frmjoblist
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmjoblist));
            this.btnClear = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.dttourlist = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.jobno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jobDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Deldate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Customer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtSearch = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dttourlist)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnClear.Location = new System.Drawing.Point(493, 30);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(98, 23);
            this.btnClear.TabIndex = 87;
            this.btnClear.Text = "Clear Search";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Location = new System.Drawing.Point(6, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 83;
            this.label6.Text = "Search By";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Location = new System.Drawing.Point(244, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 84;
            this.label5.Text = "Search";
            // 
            // cmbSearchby
            // 
            this.cmbSearchby.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchby.FormattingEnabled = true;
            this.cmbSearchby.Items.AddRange(new object[] {
            "JobNo",
            "Customer"});
            this.cmbSearchby.Location = new System.Drawing.Point(76, 30);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(160, 21);
            this.cmbSearchby.TabIndex = 81;
            // 
            // dttourlist
            // 
            this.dttourlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dttourlist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.jobno,
            this.jobDate,
            this.Deldate,
            this.Customer});
            this.dttourlist.Location = new System.Drawing.Point(0, 58);
            this.dttourlist.Name = "dttourlist";
            this.dttourlist.Size = new System.Drawing.Size(593, 459);
            this.dttourlist.TabIndex = 80;
            this.dttourlist.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dttourlist_CellDoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(598, 25);
            this.toolStrip1.TabIndex = 88;
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
            // jobno
            // 
            this.jobno.HeaderText = "Job No";
            this.jobno.Name = "jobno";
            this.jobno.ReadOnly = true;
            this.jobno.Width = 150;
            // 
            // jobDate
            // 
            this.jobDate.HeaderText = "Job Date";
            this.jobDate.Name = "jobDate";
            this.jobDate.ReadOnly = true;
            // 
            // Deldate
            // 
            this.Deldate.HeaderText = "Delivery Date";
            this.Deldate.Name = "Deldate";
            // 
            // Customer
            // 
            this.Customer.HeaderText = "Customer";
            this.Customer.Name = "Customer";
            this.Customer.ReadOnly = true;
            this.Customer.Width = 200;
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Location = new System.Drawing.Point(300, 30);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(145, 20);
            this.txtSearch.TabIndex = 82;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // frmjoblist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 528);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.dttourlist);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmjoblist";
            this.Text = "Job List";
            this.Load += new System.EventHandler(this.frmjoblist_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dttourlist)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSearchby;
        private System.Windows.Forms.DataGridView dttourlist;
        private System.Windows.Forms.DataGridViewTextBoxColumn jobno;
        private System.Windows.Forms.DataGridViewTextBoxColumn jobDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Deldate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Customer;
        private System.Windows.Forms.TextBox txtSearch;
    }
}
namespace UserAutherization
{
    partial class frmInvoiceList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInvoiceList));
            this.dgvinvoiceList = new System.Windows.Forms.DataGridView();
            this.DDeliveryNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Customer_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoNos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSearchby = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnclose = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvinvoiceList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvinvoiceList
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LemonChiffon;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvinvoiceList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvinvoiceList.BackgroundColor = System.Drawing.Color.LemonChiffon;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvinvoiceList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvinvoiceList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvinvoiceList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DDeliveryNo,
            this.Customer_ID,
            this.Date,
            this.TotalAmount,
            this.SoNos});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvinvoiceList.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvinvoiceList.Location = new System.Drawing.Point(8, 50);
            this.dgvinvoiceList.Name = "dgvinvoiceList";
            this.dgvinvoiceList.RowHeadersVisible = false;
            this.dgvinvoiceList.Size = new System.Drawing.Size(752, 483);
            this.dgvinvoiceList.TabIndex = 50;
            this.dgvinvoiceList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvinvoiceList_CellDoubleClick);
            // 
            // DDeliveryNo
            // 
            this.DDeliveryNo.HeaderText = "Invoice No";
            this.DDeliveryNo.Name = "DDeliveryNo";
            this.DDeliveryNo.Width = 125;
            // 
            // Customer_ID
            // 
            this.Customer_ID.HeaderText = "Customer ID";
            this.Customer_ID.Name = "Customer_ID";
            this.Customer_ID.Width = 125;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.Width = 125;
            // 
            // TotalAmount
            // 
            this.TotalAmount.HeaderText = "TotalAmount";
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.Width = 125;
            // 
            // SoNos
            // 
            this.SoNos.HeaderText = "Sales Order Numbers";
            this.SoNos.Name = "SoNos";
            this.SoNos.Width = 250;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(74, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 20);
            this.label6.TabIndex = 48;
            this.label6.Text = "Search By";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(314, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 20);
            this.label5.TabIndex = 49;
            this.label5.Text = "Search";
            // 
            // cmbSearchby
            // 
            this.cmbSearchby.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSearchby.FormattingEnabled = true;
            this.cmbSearchby.Items.AddRange(new object[] {
            "INV NO",
            "Customer ID"});
            this.cmbSearchby.Location = new System.Drawing.Point(164, 14);
            this.cmbSearchby.Name = "cmbSearchby";
            this.cmbSearchby.Size = new System.Drawing.Size(142, 23);
            this.cmbSearchby.TabIndex = 46;
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtSearch.Location = new System.Drawing.Point(383, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(170, 20);
            this.txtSearch.TabIndex = 47;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btnclose
            // 
            this.btnclose.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnclose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnclose.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclose.Image = global::UserAutherization.Properties.Resources.close1;
            this.btnclose.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnclose.Location = new System.Drawing.Point(8, 6);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(45, 35);
            this.btnclose.TabIndex = 51;
            this.btnclose.Text = "Close";
            this.btnclose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnclose.UseVisualStyleBackColor = false;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.PeachPuff;
            this.btnClear.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(619, 12);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(108, 23);
            this.btnClear.TabIndex = 52;
            this.btnClear.Text = "Clear Search";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // frmInvoiceList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(769, 539);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnclose);
            this.Controls.Add(this.dgvinvoiceList);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSearchby);
            this.Controls.Add(this.txtSearch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(215, 130);
            this.Name = "frmInvoiceList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Invoice List";
            this.Load += new System.EventHandler(this.frmInvoiceList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvinvoiceList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.DataGridView dgvinvoiceList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSearchby;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridViewTextBoxColumn DDeliveryNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Customer_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoNos;
        private System.Windows.Forms.Button btnClear;
    }
}
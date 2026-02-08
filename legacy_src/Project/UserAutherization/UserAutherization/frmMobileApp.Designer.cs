namespace UserAutherization
{
    partial class frmMobileApp
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMobileApp));
            this.dgvSearchCustomer = new System.Windows.Forms.DataGridView();
            this.txtItemIDSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CusID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CusName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NIC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.brand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qoh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rate3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Price3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnViewUnitCost = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchCustomer)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSearchCustomer
            // 
            this.dgvSearchCustomer.AllowUserToAddRows = false;
            this.dgvSearchCustomer.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSearchCustomer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearchCustomer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dgvSearchCustomer.BackgroundColor = System.Drawing.Color.Silver;
            this.dgvSearchCustomer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CusID,
            this.CusName,
            this.NIC,
            this.brand,
            this.qoh,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Rate3,
            this.Price3});
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchCustomer.DefaultCellStyle = dataGridViewCellStyle10;
            this.dgvSearchCustomer.Location = new System.Drawing.Point(12, 52);
            this.dgvSearchCustomer.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.dgvSearchCustomer.Name = "dgvSearchCustomer";
            this.dgvSearchCustomer.ReadOnly = true;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchCustomer.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dgvSearchCustomer.RowHeadersWidth = 25;
            this.dgvSearchCustomer.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSearchCustomer.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvSearchCustomer.Size = new System.Drawing.Size(976, 267);
            this.dgvSearchCustomer.TabIndex = 95;
            // 
            // txtItemIDSearch
            // 
            this.txtItemIDSearch.BackColor = System.Drawing.Color.Silver;
            this.txtItemIDSearch.Location = new System.Drawing.Point(87, 12);
            this.txtItemIDSearch.Name = "txtItemIDSearch";
            this.txtItemIDSearch.Size = new System.Drawing.Size(899, 20);
            this.txtItemIDSearch.TabIndex = 100;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Silver;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 15);
            this.label2.TabIndex = 99;
            this.label2.Text = "SEARCH :";
            // 
            // CusID
            // 
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.CusID.DefaultCellStyle = dataGridViewCellStyle2;
            this.CusID.HeaderText = "Item ID";
            this.CusID.Name = "CusID";
            this.CusID.ReadOnly = true;
            this.CusID.Visible = false;
            this.CusID.Width = 58;
            // 
            // CusName
            // 
            this.CusName.HeaderText = "DESCRIPTION";
            this.CusName.Name = "CusName";
            this.CusName.ReadOnly = true;
            this.CusName.Width = 300;
            // 
            // NIC
            // 
            this.NIC.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NIC.HeaderText = "Category";
            this.NIC.Name = "NIC";
            this.NIC.ReadOnly = true;
            this.NIC.Visible = false;
            // 
            // brand
            // 
            this.brand.HeaderText = "BRAND";
            this.brand.Name = "brand";
            this.brand.ReadOnly = true;
            // 
            // qoh
            // 
            this.qoh.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = null;
            this.qoh.DefaultCellStyle = dataGridViewCellStyle3;
            this.qoh.FillWeight = 428.5714F;
            this.qoh.HeaderText = "QOH";
            this.qoh.Name = "qoh";
            this.qoh.ReadOnly = true;
            this.qoh.Width = 50;
            // 
            // Column1
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column1.FillWeight = 34.28571F;
            this.Column1.HeaderText = "R.QTY";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 50;
            // 
            // Column2
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle5;
            this.Column2.FillWeight = 34.28571F;
            this.Column2.HeaderText = "COST";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 90;
            // 
            // Column3
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle6;
            this.Column3.FillWeight = 34.28571F;
            this.Column3.HeaderText = "PRICE 1";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 90;
            // 
            // Column4
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle7;
            this.Column4.FillWeight = 34.28571F;
            this.Column4.HeaderText = "PRICE 2";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 90;
            // 
            // Column5
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column5.DefaultCellStyle = dataGridViewCellStyle8;
            this.Column5.FillWeight = 34.28571F;
            this.Column5.HeaderText = "PRICE 3";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 90;
            // 
            // Rate3
            // 
            this.Rate3.HeaderText = "SHOW";
            this.Rate3.Name = "Rate3";
            this.Rate3.ReadOnly = true;
            this.Rate3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Rate3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Rate3.Width = 80;
            // 
            // Price3
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Price3.DefaultCellStyle = dataGridViewCellStyle9;
            this.Price3.HeaderText = "PRICE";
            this.Price3.Name = "Price3";
            this.Price3.ReadOnly = true;
            this.Price3.Visible = false;
            this.Price3.Width = 80;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 336);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(721, 135);
            this.richTextBox1.TabIndex = 101;
            this.richTextBox1.Text = "Your Selected Items will appear here and system facilitate to copy";
            // 
            // btnViewUnitCost
            // 
            this.btnViewUnitCost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnViewUnitCost.ForeColor = System.Drawing.Color.White;
            this.btnViewUnitCost.Location = new System.Drawing.Point(739, 336);
            this.btnViewUnitCost.Name = "btnViewUnitCost";
            this.btnViewUnitCost.Size = new System.Drawing.Size(91, 23);
            this.btnViewUnitCost.TabIndex = 102;
            this.btnViewUnitCost.Text = "COPY";
            this.btnViewUnitCost.UseVisualStyleBackColor = false;
            // 
            // frmMobileApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1000, 483);
            this.Controls.Add(this.btnViewUnitCost);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.txtItemIDSearch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvSearchCustomer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMobileApp";
            this.Text = "PRICE INFORMATION";
            this.Load += new System.EventHandler(this.frmMobileApp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchCustomer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSearchCustomer;
        private System.Windows.Forms.TextBox txtItemIDSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn CusID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CusName;
        private System.Windows.Forms.DataGridViewTextBoxColumn NIC;
        private System.Windows.Forms.DataGridViewTextBoxColumn brand;
        private System.Windows.Forms.DataGridViewTextBoxColumn qoh;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Rate3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnViewUnitCost;
    }
}
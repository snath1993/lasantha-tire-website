namespace UserAutherization
{
    partial class frmWarehouseSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWarehouseSearch));
            this.dgvSearch = new System.Windows.Forms.DataGridView();
            this.txtSearchBy = new System.Windows.Forms.TextBox();
            this.cmbSearchBy = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnclose = new System.Windows.Forms.Button();
            this.WuseId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WuseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSearch
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvSearch.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearch.BackgroundColor = System.Drawing.Color.White;
            this.dgvSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WuseId,
            this.WuseName,
            this.Address});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearch.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSearch.Location = new System.Drawing.Point(9, 50);
            this.dgvSearch.Name = "dgvSearch";
            this.dgvSearch.RowHeadersVisible = false;
            this.dgvSearch.Size = new System.Drawing.Size(555, 297);
            this.dgvSearch.TabIndex = 0;
            this.dgvSearch.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearch_CellDoubleClick);
            // 
            // txtSearchBy
            // 
            this.txtSearchBy.Location = new System.Drawing.Point(392, 20);
            this.txtSearchBy.Name = "txtSearchBy";
            this.txtSearchBy.Size = new System.Drawing.Size(172, 20);
            this.txtSearchBy.TabIndex = 6;
            this.txtSearchBy.TextChanged += new System.EventHandler(this.txtSearchBy_TextChanged);
            // 
            // cmbSearchBy
            // 
            this.cmbSearchBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchBy.FormattingEnabled = true;
            this.cmbSearchBy.Items.AddRange(new object[] {
            "Warehouse Id",
            "Warehouse Name"});
            this.cmbSearchBy.Location = new System.Drawing.Point(150, 20);
            this.cmbSearchBy.Name = "cmbSearchBy";
            this.cmbSearchBy.Size = new System.Drawing.Size(154, 21);
            this.cmbSearchBy.TabIndex = 7;
            this.cmbSearchBy.SelectedIndexChanged += new System.EventHandler(this.cmbSearchBy_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(62, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 20);
            this.label6.TabIndex = 57;
            this.label6.Text = "Search By";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(310, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 20);
            this.label5.TabIndex = 58;
            this.label5.Text = "Search";
            // 
            // btnclose
            // 
            this.btnclose.BackColor = System.Drawing.Color.White;
            this.btnclose.BackgroundImage = global::UserAutherization.Properties.Resources.Close;
            this.btnclose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnclose.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnclose.Location = new System.Drawing.Point(12, 9);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(45, 35);
            this.btnclose.TabIndex = 59;
            this.btnclose.Text = "Close";
            this.btnclose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnclose.UseVisualStyleBackColor = false;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // WuseId
            // 
            this.WuseId.HeaderText = "Warehouse ID";
            this.WuseId.Name = "WuseId";
            this.WuseId.Width = 150;
            // 
            // WuseName
            // 
            this.WuseName.HeaderText = "Description";
            this.WuseName.Name = "WuseName";
            this.WuseName.Width = 200;
            // 
            // Address
            // 
            this.Address.HeaderText = "Address";
            this.Address.Name = "Address";
            this.Address.Width = 200;
            // 
            // frmWarehouseSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(572, 361);
            this.Controls.Add(this.dgvSearch);
            this.Controls.Add(this.btnclose);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSearchBy);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSearchBy);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 50);
            this.MaximizeBox = false;
            this.Name = "frmWarehouseSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Warehouse List";
            this.Load += new System.EventHandler(this.frmWarehouseSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearchBy;
        private System.Windows.Forms.ComboBox cmbSearchBy;
        private System.Windows.Forms.DataGridView dgvSearch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.DataGridViewTextBoxColumn WuseId;
        private System.Windows.Forms.DataGridViewTextBoxColumn WuseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
    }
}
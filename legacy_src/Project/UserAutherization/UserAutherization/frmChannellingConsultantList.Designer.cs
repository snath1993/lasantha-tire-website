namespace UserAutherization
{
    partial class frmChannellingConsultantList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChannellingConsultantList));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.txtsearch = new System.Windows.Forms.TextBox();
            this.dgvConsultants = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConsultants)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.cmbType);
            this.groupBox1.Controls.Add(this.txtsearch);
            this.groupBox1.Controls.Add(this.dgvConsultants);
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(11, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(505, 507);
            this.groupBox1.TabIndex = 147;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Consultant Master";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(237, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 14);
            this.label1.TabIndex = 152;
            this.label1.Text = "Search";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.BackColor = System.Drawing.Color.Transparent;
            this.lblDate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.Black;
            this.lblDate.Location = new System.Drawing.Point(6, 29);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(69, 14);
            this.lblDate.TabIndex = 151;
            this.lblDate.Text = "Search By";
            // 
            // cmbType
            // 
            this.cmbType.BackColor = System.Drawing.Color.White;
            this.cmbType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbType.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbType.ForeColor = System.Drawing.Color.Black;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Code",
            "Name"});
            this.cmbType.Location = new System.Drawing.Point(79, 25);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(152, 23);
            this.cmbType.TabIndex = 14;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // txtsearch
            // 
            this.txtsearch.BackColor = System.Drawing.Color.White;
            this.txtsearch.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsearch.ForeColor = System.Drawing.Color.Black;
            this.txtsearch.Location = new System.Drawing.Point(304, 24);
            this.txtsearch.Name = "txtsearch";
            this.txtsearch.Size = new System.Drawing.Size(172, 23);
            this.txtsearch.TabIndex = 0;
            this.txtsearch.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // dgvConsultants
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvConsultants.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvConsultants.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvConsultants.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvConsultants.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvConsultants.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConsultants.Location = new System.Drawing.Point(6, 55);
            this.dgvConsultants.Name = "dgvConsultants";
            this.dgvConsultants.RowHeadersVisible = false;
            this.dgvConsultants.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConsultants.Size = new System.Drawing.Size(482, 442);
            this.dgvConsultants.TabIndex = 1;
            this.dgvConsultants.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConsultants_CellDoubleClick);
            // 
            // frmChannellingConsultantList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 536);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmChannellingConsultantList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Consultant List ";
            this.Load += new System.EventHandler(this.frmChannellingConsultantList_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConsultants)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.TextBox txtsearch;
        private System.Windows.Forms.DataGridView dgvConsultants;
    }
}
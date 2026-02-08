namespace UserAutherization
{
    partial class frmSelectConsult
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
            this.dgvConsultants = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConsultants)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            this.dgvConsultants.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConsultants_CellContentClick);
            this.dgvConsultants.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConsultantRoom_CellDoubleClick);
            this.dgvConsultants.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvConsultantRoom_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.cmbType);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.dgvConsultants);
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(12, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(505, 507);
            this.groupBox1.TabIndex = 1;
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
            this.lblDate.Click += new System.EventHandler(this.lblDate_Click);
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
            "Name",
            "Type"});
            this.cmbType.Location = new System.Drawing.Point(79, 25);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(152, 23);
            this.cmbType.TabIndex = 14;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.Black;
            this.textBox1.Location = new System.Drawing.Point(304, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(172, 23);
            this.textBox1.TabIndex = 0;
            this.textBox1.TabIndexChanged += new System.EventHandler(this.textBox1_TabIndexChanged);
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.LightGray;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(318, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 20);
            this.label3.TabIndex = 146;
            this.label3.Text = "Maintain Master Data";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // frmSelectConsult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(528, 552);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmSelectConsult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Consultant List";
            this.Load += new System.EventHandler(this.frmSelectConsult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConsultants)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvConsultants;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}
namespace UserAutherization
{
    partial class frmCustomMasterList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCustomMasterList));
            this.dgvSearchCustomer = new System.Windows.Forms.DataGridView();
            this.CusID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchCustomer)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSearchCustomer
            // 
            this.dgvSearchCustomer.AllowUserToAddRows = false;
            this.dgvSearchCustomer.AllowUserToDeleteRows = false;
            this.dgvSearchCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchCustomer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CusID,
            this.description});
            this.dgvSearchCustomer.Location = new System.Drawing.Point(79, 44);
            this.dgvSearchCustomer.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.dgvSearchCustomer.Name = "dgvSearchCustomer";
            this.dgvSearchCustomer.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSearchCustomer.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSearchCustomer.RowHeadersVisible = false;
            this.dgvSearchCustomer.Size = new System.Drawing.Size(454, 407);
            this.dgvSearchCustomer.TabIndex = 95;
            this.dgvSearchCustomer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchCustomer_CellDoubleClick);
            // 
            // CusID
            // 
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.CusID.DefaultCellStyle = dataGridViewCellStyle1;
            this.CusID.HeaderText = "ID";
            this.CusID.Name = "CusID";
            this.CusID.ReadOnly = true;
            this.CusID.Width = 150;
            // 
            // description
            // 
            this.description.HeaderText = "Description";
            this.description.Name = "description";
            this.description.ReadOnly = true;
            this.description.Width = 300;
            // 
            // frmCustomMasterList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 465);
            this.Controls.Add(this.dgvSearchCustomer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCustomMasterList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCustomMasterList";
            this.Load += new System.EventHandler(this.frmCustomMasterList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchCustomer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSearchCustomer;
        private System.Windows.Forms.DataGridViewTextBoxColumn CusID;
        private System.Windows.Forms.DataGridViewTextBoxColumn description;
    }
}
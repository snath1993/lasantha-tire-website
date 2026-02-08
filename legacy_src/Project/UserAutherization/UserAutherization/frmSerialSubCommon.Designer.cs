namespace UserAutherization
{
    partial class frmSerialSubCommon
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtWarehouseID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtItemID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSerialCount = new System.Windows.Forms.Label();
            this.dgvSerialNum = new System.Windows.Forms.DataGridView();
            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colSN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSerialEnter = new System.Windows.Forms.TextBox();
            this.btn_Add = new System.Windows.Forms.Button();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSerialNum)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtWarehouseID);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtDescription);
            this.panel1.Controls.Add(this.txtItemID);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblSerialCount);
            this.panel1.Controls.Add(this.dgvSerialNum);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtSerialEnter);
            this.panel1.Controls.Add(this.btn_Add);
            this.panel1.Location = new System.Drawing.Point(5, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 458);
            this.panel1.TabIndex = 96;
            // 
            // txtWarehouseID
            // 
            this.txtWarehouseID.BackColor = System.Drawing.Color.White;
            this.txtWarehouseID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWarehouseID.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWarehouseID.Location = new System.Drawing.Point(118, 9);
            this.txtWarehouseID.Name = "txtWarehouseID";
            this.txtWarehouseID.ReadOnly = true;
            this.txtWarehouseID.Size = new System.Drawing.Size(272, 21);
            this.txtWarehouseID.TabIndex = 108;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 107;
            this.label1.Text = "Warehouse ID";
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.Color.White;
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescription.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescription.Location = new System.Drawing.Point(118, 62);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.Size = new System.Drawing.Size(272, 21);
            this.txtDescription.TabIndex = 106;
            // 
            // txtItemID
            // 
            this.txtItemID.BackColor = System.Drawing.Color.White;
            this.txtItemID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtItemID.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtItemID.Location = new System.Drawing.Point(118, 35);
            this.txtItemID.Name = "txtItemID";
            this.txtItemID.ReadOnly = true;
            this.txtItemID.Size = new System.Drawing.Size(272, 21);
            this.txtItemID.TabIndex = 105;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 104;
            this.label3.Text = "Description";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 103;
            this.label2.Text = "Item ID ";
            // 
            // lblSerialCount
            // 
            this.lblSerialCount.AutoSize = true;
            this.lblSerialCount.Location = new System.Drawing.Point(13, 434);
            this.lblSerialCount.Name = "lblSerialCount";
            this.lblSerialCount.Size = new System.Drawing.Size(58, 13);
            this.lblSerialCount.TabIndex = 102;
            this.lblSerialCount.Text = ".................";
            // 
            // dgvSerialNum
            // 
            this.dgvSerialNum.AllowUserToAddRows = false;
            this.dgvSerialNum.AllowUserToDeleteRows = false;
            this.dgvSerialNum.AllowUserToResizeColumns = false;
            this.dgvSerialNum.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvSerialNum.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSerialNum.BackgroundColor = System.Drawing.Color.White;
            this.dgvSerialNum.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvSerialNum.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSerialNum.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSerialNum.ColumnHeadersHeight = 20;
            this.dgvSerialNum.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelect,
            this.colSN,
            this.colStatus});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSerialNum.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSerialNum.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvSerialNum.GridColor = System.Drawing.Color.Silver;
            this.dgvSerialNum.Location = new System.Drawing.Point(10, 148);
            this.dgvSerialNum.Name = "dgvSerialNum";
            this.dgvSerialNum.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSerialNum.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSerialNum.RowHeadersVisible = false;
            this.dgvSerialNum.RowHeadersWidth = 22;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSerialNum.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSerialNum.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvSerialNum.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSerialNum.Size = new System.Drawing.Size(382, 285);
            this.dgvSerialNum.TabIndex = 99;
            // 
            // colSelect
            // 
            this.colSelect.HeaderText = "Select";
            this.colSelect.Name = "colSelect";
            this.colSelect.Width = 75;
            // 
            // colSN
            // 
            this.colSN.HeaderText = "Serilal Number";
            this.colSN.Name = "colSN";
            this.colSN.ReadOnly = true;
            this.colSN.Width = 200;
            // 
            // colStatus
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colStatus.DefaultCellStyle = dataGridViewCellStyle3;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 105;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(7, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 13);
            this.label4.TabIndex = 92;
            this.label4.Text = "Type Serial Number To Find";
            // 
            // txtSerialEnter
            // 
            this.txtSerialEnter.BackColor = System.Drawing.Color.White;
            this.txtSerialEnter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSerialEnter.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerialEnter.Location = new System.Drawing.Point(6, 113);
            this.txtSerialEnter.Name = "txtSerialEnter";
            this.txtSerialEnter.Size = new System.Drawing.Size(316, 21);
            this.txtSerialEnter.TabIndex = 93;
            // 
            // btn_Add
            // 
            this.btn_Add.BackColor = System.Drawing.Color.White;
            this.btn_Add.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.btn_Add.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Add.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Add.ForeColor = System.Drawing.Color.Black;
            this.btn_Add.Location = new System.Drawing.Point(328, 112);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(65, 23);
            this.btn_Add.TabIndex = 94;
            this.btn_Add.Text = "Find";
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // btn_Clear
            // 
            this.btn_Clear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Clear.ForeColor = System.Drawing.Color.Black;
            this.btn_Clear.Location = new System.Drawing.Point(262, 472);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(67, 23);
            this.btn_Clear.TabIndex = 109;
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.UseVisualStyleBackColor = false;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // save
            // 
            this.save.BackColor = System.Drawing.Color.WhiteSmoke;
            this.save.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.save.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.save.ForeColor = System.Drawing.Color.Black;
            this.save.Location = new System.Drawing.Point(199, 472);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(58, 23);
            this.save.TabIndex = 108;
            this.save.Text = "OK";
            this.save.UseVisualStyleBackColor = false;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Cancel.ForeColor = System.Drawing.Color.Black;
            this.btn_Cancel.Location = new System.Drawing.Point(335, 472);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(66, 23);
            this.btn_Cancel.TabIndex = 107;
            this.btn_Cancel.Text = "Close";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // frmSerialSubCommon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(411, 499);
            this.Controls.Add(this.btn_Clear);
            this.Controls.Add(this.save);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_Cancel);
            this.Name = "frmSerialSubCommon";
            this.Text = "Serial Number Selection";
            this.Load += new System.EventHandler(this.frmSerialSubCommon_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSerialNum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtItemID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSerialCount;
        private System.Windows.Forms.DataGridView dgvSerialNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSerialEnter;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSN;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.TextBox txtWarehouseID;
        private System.Windows.Forms.Label label1;
    }
}
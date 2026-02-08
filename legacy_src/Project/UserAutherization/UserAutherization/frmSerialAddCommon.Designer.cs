namespace UserAutherization
{
    partial class frmSerialAddCommon
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btn_Add = new System.Windows.Forms.Button();
            this.txtSerialEnter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtItemID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSerialCount = new System.Windows.Forms.Label();
            this.dgvSerialNum = new System.Windows.Forms.DataGridView();
            this.colSN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSerialNum)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Delete
            // 
            this.btn_Delete.BackColor = System.Drawing.Color.White;
            this.btn_Delete.Enabled = false;
            this.btn_Delete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Delete.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Delete.ForeColor = System.Drawing.Color.Black;
            this.btn_Delete.Location = new System.Drawing.Point(328, 112);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(65, 23);
            this.btn_Delete.TabIndex = 98;
            this.btn_Delete.Text = "Remove";
            this.btn_Delete.UseVisualStyleBackColor = false;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(118, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(163, 13);
            this.label5.TabIndex = 97;
            this.label5.Text = "Consecutive Serial Number";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.White;
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(64, 115);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(48, 21);
            this.numericUpDown1.TabIndex = 96;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.White;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.ForeColor = System.Drawing.Color.Black;
            this.checkBox1.Location = new System.Drawing.Point(6, 118);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(54, 18);
            this.checkBox1.TabIndex = 95;
            this.checkBox1.Text = "Add";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btn_Add
            // 
            this.btn_Add.BackColor = System.Drawing.Color.White;
            this.btn_Add.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.btn_Add.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Add.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Add.ForeColor = System.Drawing.Color.Black;
            this.btn_Add.Location = new System.Drawing.Point(328, 85);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(65, 23);
            this.btn_Add.TabIndex = 94;
            this.btn_Add.Text = "Add";
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // txtSerialEnter
            // 
            this.txtSerialEnter.BackColor = System.Drawing.Color.White;
            this.txtSerialEnter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSerialEnter.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerialEnter.Location = new System.Drawing.Point(6, 86);
            this.txtSerialEnter.Name = "txtSerialEnter";
            this.txtSerialEnter.Size = new System.Drawing.Size(316, 21);
            this.txtSerialEnter.TabIndex = 93;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(7, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(178, 13);
            this.label4.TabIndex = 92;
            this.label4.Text = "Enter a Serial Number To Add";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtDescription);
            this.panel1.Controls.Add(this.txtItemID);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblSerialCount);
            this.panel1.Controls.Add(this.dgvSerialNum);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.btn_Delete);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtSerialEnter);
            this.panel1.Controls.Add(this.btn_Add);
            this.panel1.Location = new System.Drawing.Point(5, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 458);
            this.panel1.TabIndex = 95;
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.Color.White;
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescription.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescription.Location = new System.Drawing.Point(92, 35);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.Size = new System.Drawing.Size(301, 21);
            this.txtDescription.TabIndex = 106;
            // 
            // txtItemID
            // 
            this.txtItemID.BackColor = System.Drawing.Color.White;
            this.txtItemID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtItemID.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtItemID.Location = new System.Drawing.Point(94, 7);
            this.txtItemID.Name = "txtItemID";
            this.txtItemID.ReadOnly = true;
            this.txtItemID.Size = new System.Drawing.Size(296, 21);
            this.txtItemID.TabIndex = 105;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 104;
            this.label3.Text = "Description";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 12);
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
            this.colSN,
            this.colStatus});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSerialNum.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSerialNum.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvSerialNum.GridColor = System.Drawing.Color.Silver;
            this.dgvSerialNum.Location = new System.Drawing.Point(10, 144);
            this.dgvSerialNum.Name = "dgvSerialNum";
            this.dgvSerialNum.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSerialNum.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSerialNum.RowHeadersVisible = false;
            this.dgvSerialNum.RowHeadersWidth = 22;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSerialNum.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSerialNum.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvSerialNum.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSerialNum.Size = new System.Drawing.Size(382, 285);
            this.dgvSerialNum.TabIndex = 99;
            this.dgvSerialNum.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvSerialNum_CellBeginEdit);
            this.dgvSerialNum.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSerialNum_RowEnter);
            // 
            // colSN
            // 
            this.colSN.HeaderText = "Serilal Number";
            this.colSN.Name = "colSN";
            this.colSN.ReadOnly = true;
            this.colSN.Width = 227;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 150;
            // 
            // btn_Clear
            // 
            this.btn_Clear.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Clear.ForeColor = System.Drawing.Color.Black;
            this.btn_Clear.Location = new System.Drawing.Point(266, 471);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(67, 23);
            this.btn_Clear.TabIndex = 102;
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
            this.save.Location = new System.Drawing.Point(203, 471);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(58, 23);
            this.save.TabIndex = 101;
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
            this.btn_Cancel.Location = new System.Drawing.Point(339, 471);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(66, 23);
            this.btn_Cancel.TabIndex = 100;
            this.btn_Cancel.Text = "Close";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // frmSerialAddCommon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(411, 499);
            this.Controls.Add(this.btn_Clear);
            this.Controls.Add(this.save);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.panel1);
            this.Name = "frmSerialAddCommon";
            this.Text = "Serial Number Entry";
            this.Load += new System.EventHandler(this.frmSerialAddCommon_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSerialNum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txtSerialEnter;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvSerialNum;
        private System.Windows.Forms.Label lblSerialCount;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtItemID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSN;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
    }
}
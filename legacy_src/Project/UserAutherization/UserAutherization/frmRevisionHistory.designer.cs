namespace UserAutherization
{
    partial class frmRevisionHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRevisionHistory));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbRevNo = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnSetup = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.cmbDocType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvRevisionHistry = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRefCode = new System.Windows.Forms.TextBox();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRevisionHistry)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Revision No";
            // 
            // cmbRevNo
            // 
            this.cmbRevNo.FormattingEnabled = true;
            this.cmbRevNo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbRevNo.Location = new System.Drawing.Point(96, 44);
            this.cmbRevNo.Name = "cmbRevNo";
            this.cmbRevNo.Size = new System.Drawing.Size(125, 21);
            this.cmbRevNo.TabIndex = 126;
            this.cmbRevNo.SelectedIndexChanged += new System.EventHandler(this.cmbRevNo_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel3.Controls.Add(this.btnSetup);
            this.panel3.Controls.Add(this.btnClear);
            this.panel3.Controls.Add(this.btnView);
            this.panel3.Controls.Add(this.button4);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.btnSave);
            this.panel3.Controls.Add(this.btnPrint);
            this.panel3.Controls.Add(this.btnDelete);
            this.panel3.Location = new System.Drawing.Point(7, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(838, 26);
            this.panel3.TabIndex = 127;
            // 
            // btnSetup
            // 
            this.btnSetup.BackColor = System.Drawing.SystemColors.Window;
            this.btnSetup.Enabled = false;
            this.btnSetup.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSetup.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetup.Image = ((System.Drawing.Image)(resources.GetObject("btnSetup.Image")));
            this.btnSetup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSetup.Location = new System.Drawing.Point(299, 2);
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.Size = new System.Drawing.Size(65, 21);
            this.btnSetup.TabIndex = 71;
            this.btnSetup.TabStop = false;
            this.btnSetup.Text = "Setup";
            this.btnSetup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSetup.UseVisualStyleBackColor = false;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.SystemColors.Window;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClear.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClear.Location = new System.Drawing.Point(234, 2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(65, 21);
            this.btnClear.TabIndex = 70;
            this.btnClear.Text = "Clear";
            this.btnClear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.SystemColors.Window;
            this.btnView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnView.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnView.Image = ((System.Drawing.Image)(resources.GetObject("btnView.Image")));
            this.btnView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnView.Location = new System.Drawing.Point(178, 2);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(56, 21);
            this.btnView.TabIndex = 66;
            this.btnView.Text = "View";
            this.btnView.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.Window;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button4.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.Location = new System.Drawing.Point(364, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(74, 21);
            this.button4.TabIndex = 63;
            this.button4.Text = "Refresh";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.UseVisualStyleBackColor = false;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(648, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(183, 26);
            this.label10.TabIndex = 5;
            this.label10.Text = "Revision History";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.Window;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(53, 21);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "OK";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.Window;
            this.btnPrint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrint.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(122, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(56, 21);
            this.btnPrint.TabIndex = 58;
            this.btnPrint.Text = "Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.Window;
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDelete.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(56, 2);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(66, 21);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Cancel";
            this.btnDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // cmbDocType
            // 
            this.cmbDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDocType.FormattingEnabled = true;
            this.cmbDocType.Items.AddRange(new object[] {
            "Quotation",
            "Service Agreement",
            "Estimate"});
            this.cmbDocType.Location = new System.Drawing.Point(626, 44);
            this.cmbDocType.Name = "cmbDocType";
            this.cmbDocType.Size = new System.Drawing.Size(125, 21);
            this.cmbDocType.TabIndex = 129;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(523, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 128;
            this.label2.Text = "Document Type";
            // 
            // dgvRevisionHistry
            // 
            this.dgvRevisionHistry.AllowUserToAddRows = false;
            this.dgvRevisionHistry.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MintCream;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvRevisionHistry.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRevisionHistry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRevisionHistry.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvRevisionHistry.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvRevisionHistry.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRevisionHistry.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRevisionHistry.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRevisionHistry.GridColor = System.Drawing.Color.WhiteSmoke;
            this.dgvRevisionHistry.Location = new System.Drawing.Point(10, 81);
            this.dgvRevisionHistry.Name = "dgvRevisionHistry";
            this.dgvRevisionHistry.ReadOnly = true;
            this.dgvRevisionHistry.RowHeadersVisible = false;
            this.dgvRevisionHistry.ShowEditingIcon = false;
            this.dgvRevisionHistry.Size = new System.Drawing.Size(835, 512);
            this.dgvRevisionHistry.TabIndex = 130;
            this.dgvRevisionHistry.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRevisionHistry_CellContentDoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(251, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 131;
            this.label3.Text = "Referance No";
            // 
            // txtRefCode
            // 
            this.txtRefCode.Location = new System.Drawing.Point(340, 44);
            this.txtRefCode.Name = "txtRefCode";
            this.txtRefCode.Size = new System.Drawing.Size(146, 21);
            this.txtRefCode.TabIndex = 132;
            this.txtRefCode.TextChanged += new System.EventHandler(this.txtRefCode_TextChanged);
            // 
            // frmRevisionHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(857, 605);
            this.Controls.Add(this.txtRefCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvRevisionHistry);
            this.Controls.Add(this.cmbDocType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.cmbRevNo);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmRevisionHistory";
            this.Text = "Revision History";
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRevisionHistry)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRevNo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ComboBox cmbDocType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnSetup;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGridView dgvRevisionHistry;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRefCode;
    }
}
namespace Perfect_Hospital_Management_System
{
    partial class frmChannelingList
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChannelingList));
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            ""}, -1);
            this.grptblChannelingList = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.dgvChannelingList = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cmbSession = new System.Windows.Forms.ComboBox();
            this.dtpDateFrom = new System.Windows.Forms.DateTimePicker();
            this.lblSession = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.txtRoom = new System.Windows.Forms.TextBox();
            this.lblRoom = new System.Windows.Forms.Label();
            this.lblConsultantRoom = new System.Windows.Forms.Label();
            this.btnConsultant = new System.Windows.Forms.Button();
            this.txtConsultant = new System.Windows.Forms.TextBox();
            this.lstvConsultant = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.grptblChannelingList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChannelingList)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grptblChannelingList
            // 
            this.grptblChannelingList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grptblChannelingList.BackColor = System.Drawing.Color.Gainsboro;
            this.grptblChannelingList.Controls.Add(this.btnOK);
            this.grptblChannelingList.Controls.Add(this.dgvChannelingList);
            this.grptblChannelingList.Location = new System.Drawing.Point(12, 122);
            this.grptblChannelingList.Name = "grptblChannelingList";
            this.grptblChannelingList.Size = new System.Drawing.Size(604, 477);
            this.grptblChannelingList.TabIndex = 188;
            this.grptblChannelingList.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(514, 448);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dgvChannelingList
            // 
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.SeaShell;
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            this.dgvChannelingList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvChannelingList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvChannelingList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChannelingList.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dgvChannelingList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChannelingList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvChannelingList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvChannelingList.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvChannelingList.GridColor = System.Drawing.Color.SeaShell;
            this.dgvChannelingList.Location = new System.Drawing.Point(14, 19);
            this.dgvChannelingList.Name = "dgvChannelingList";
            this.dgvChannelingList.RowHeadersVisible = false;
            this.dgvChannelingList.Size = new System.Drawing.Size(575, 416);
            this.dgvChannelingList.TabIndex = 188;
            this.dgvChannelingList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvtblChannelingList_CellDoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.cmbSession);
            this.groupBox1.Controls.Add(this.dtpDateFrom);
            this.groupBox1.Controls.Add(this.lblSession);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.txtRoom);
            this.groupBox1.Controls.Add(this.lblRoom);
            this.groupBox1.Controls.Add(this.lblConsultantRoom);
            this.groupBox1.Controls.Add(this.btnConsultant);
            this.groupBox1.Controls.Add(this.txtConsultant);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(604, 104);
            this.groupBox1.TabIndex = 188;
            this.groupBox1.TabStop = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(14, 72);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click_1);
            // 
            // cmbSession
            // 
            this.cmbSession.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSession.Location = new System.Drawing.Point(95, 43);
            this.cmbSession.Name = "cmbSession";
            this.cmbSession.Size = new System.Drawing.Size(185, 23);
            this.cmbSession.TabIndex = 3;
            this.cmbSession.Visible = false;
            this.cmbSession.SelectedIndexChanged += new System.EventHandler(this.cmbSession_SelectedIndexChanged);
            // 
            // dtpDateFrom
            // 
            this.dtpDateFrom.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateFrom.Location = new System.Drawing.Point(408, 17);
            this.dtpDateFrom.Name = "dtpDateFrom";
            this.dtpDateFrom.Size = new System.Drawing.Size(181, 22);
            this.dtpDateFrom.TabIndex = 2;
            this.dtpDateFrom.ValueChanged += new System.EventHandler(this.dtpDateFrom_ValueChanged);
            // 
            // lblSession
            // 
            this.lblSession.AutoSize = true;
            this.lblSession.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSession.Location = new System.Drawing.Point(14, 48);
            this.lblSession.Name = "lblSession";
            this.lblSession.Size = new System.Drawing.Size(55, 19);
            this.lblSession.TabIndex = 202;
            this.lblSession.Text = "Session";
            this.lblSession.Visible = false;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.Location = new System.Drawing.Point(326, 21);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(38, 19);
            this.lblDate.TabIndex = 201;
            this.lblDate.Text = "Date";
            // 
            // txtRoom
            // 
            this.txtRoom.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRoom.Location = new System.Drawing.Point(408, 45);
            this.txtRoom.Name = "txtRoom";
            this.txtRoom.Size = new System.Drawing.Size(181, 22);
            this.txtRoom.TabIndex = 4;
            this.txtRoom.Visible = false;
            this.txtRoom.TextChanged += new System.EventHandler(this.txtRoom_TextChanged);
            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRoom.Location = new System.Drawing.Point(327, 48);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(46, 19);
            this.lblRoom.TabIndex = 195;
            this.lblRoom.Text = "Room";
            this.lblRoom.Visible = false;
            // 
            // lblConsultantRoom
            // 
            this.lblConsultantRoom.AutoSize = true;
            this.lblConsultantRoom.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConsultantRoom.Location = new System.Drawing.Point(14, 20);
            this.lblConsultantRoom.Name = "lblConsultantRoom";
            this.lblConsultantRoom.Size = new System.Drawing.Size(73, 19);
            this.lblConsultantRoom.TabIndex = 194;
            this.lblConsultantRoom.Text = "Consultant";
            // 
            // btnConsultant
            // 
            this.btnConsultant.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConsultant.BackgroundImage")));
            this.btnConsultant.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConsultant.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConsultant.Location = new System.Drawing.Point(279, 16);
            this.btnConsultant.Name = "btnConsultant";
            this.btnConsultant.Size = new System.Drawing.Size(28, 23);
            this.btnConsultant.TabIndex = 1;
            this.btnConsultant.UseVisualStyleBackColor = true;
            this.btnConsultant.Click += new System.EventHandler(this.btnConsultant_Click_1);
            // 
            // txtConsultant
            // 
            this.txtConsultant.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsultant.Location = new System.Drawing.Point(95, 17);
            this.txtConsultant.Name = "txtConsultant";
            this.txtConsultant.Size = new System.Drawing.Size(185, 22);
            this.txtConsultant.TabIndex = 0;
            // 
            // lstvConsultant
            // 
            this.lstvConsultant.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lstvConsultant.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3});
            this.lstvConsultant.Location = new System.Drawing.Point(37, 51);
            this.lstvConsultant.Name = "lstvConsultant";
            this.lstvConsultant.Size = new System.Drawing.Size(256, 22);
            this.lstvConsultant.TabIndex = 199;
            this.lstvConsultant.UseCompatibleStateImageBehavior = false;
            this.lstvConsultant.View = System.Windows.Forms.View.Details;
            this.lstvConsultant.Visible = false;
            this.lstvConsultant.Click += new System.EventHandler(this.lstvConsultant_Click_1);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Consultant";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ConsultantCode";
            this.columnHeader2.Width = 200;
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmChannelingList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(633, 611);
            this.Controls.Add(this.lstvConsultant);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grptblChannelingList);
            this.Location = new System.Drawing.Point(25, 75);
            this.Name = "frmChannelingList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Channeling List";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.frmChannelingList_Activated);
            this.Load += new System.EventHandler(this.frmtblChannelingList_Load);
            this.grptblChannelingList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChannelingList)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grptblChannelingList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ListView lstvConsultant;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label lblSession;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.Label lblConsultantRoom;
        private System.Windows.Forms.Button btnConsultant;
        private System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.DataGridView dgvChannelingList;
        public System.Windows.Forms.ComboBox cmbSession;
        public System.Windows.Forms.DateTimePicker dtpDateFrom;
        public System.Windows.Forms.TextBox txtRoom;
        public System.Windows.Forms.TextBox txtConsultant;
        public System.Windows.Forms.Timer timer1;
    }
}
namespace UserAutherization
{
    partial class frmXray
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmXray));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvScanItems = new System.Windows.Forms.DataGridView();
            this.XrayID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScanName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GlAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConsultantFee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HospitalFee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnConsultant = new System.Windows.Forms.Button();
            this.btnScanNames = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.lblCreditCardNo = new System.Windows.Forms.Label();
            this.txtCreditCardNo = new System.Windows.Forms.TextBox();
            this.dtpDueDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.txtReceiptNo = new System.Windows.Forms.TextBox();
            this.txtTokenNo = new System.Windows.Forms.TextBox();
            this.txtScanName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.lblContactNo = new System.Windows.Forms.Label();
            this.txtConsultant = new System.Windows.Forms.TextBox();
            this.txtContactNo = new System.Windows.Forms.TextBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtConsultantFind = new System.Windows.Forms.TextBox();
            this.cmbScanSerch = new System.Windows.Forms.ComboBox();
            this.btnReprintXray = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpRepDate = new System.Windows.Forms.DateTimePicker();
            this.btnRefund = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtnetTotal = new System.Windows.Forms.TextBox();
            this.txtDisAmount = new System.Windows.Forms.TextBox();
            this.txtdisRate = new System.Windows.Forms.TextBox();
            this.rbtnboth = new System.Windows.Forms.RadioButton();
            this.rbtnDoctorfee = new System.Windows.Forms.RadioButton();
            this.rbtnHosfee = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtncash = new System.Windows.Forms.RadioButton();
            this.rbtnbht = new System.Windows.Forms.RadioButton();
            this.rbtnetuopd = new System.Windows.Forms.RadioButton();
            this.txtPatientNo = new System.Windows.Forms.TextBox();
            this.cbxInpatient = new System.Windows.Forms.CheckBox();
            this.txtScanTotal = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.dgvScanRefundSave = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScanItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScanRefundSave)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvScanItems
            // 
            this.dgvScanItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScanItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.XrayID,
            this.ScanName,
            this.GlAccount,
            this.Duration,
            this.ConsultantFee,
            this.HospitalFee,
            this.Total});
            this.dgvScanItems.Location = new System.Drawing.Point(13, 288);
            this.dgvScanItems.Name = "dgvScanItems";
            this.dgvScanItems.ReadOnly = true;
            this.dgvScanItems.RowHeadersVisible = false;
            this.dgvScanItems.Size = new System.Drawing.Size(679, 139);
            this.dgvScanItems.TabIndex = 14;
            // 
            // XrayID
            // 
            this.XrayID.HeaderText = "XrayID";
            this.XrayID.Name = "XrayID";
            this.XrayID.ReadOnly = true;
            // 
            // ScanName
            // 
            this.ScanName.HeaderText = "Xray Name";
            this.ScanName.Name = "ScanName";
            this.ScanName.ReadOnly = true;
            this.ScanName.Width = 150;
            // 
            // GlAccount
            // 
            this.GlAccount.HeaderText = "Account";
            this.GlAccount.Name = "GlAccount";
            this.GlAccount.ReadOnly = true;
            this.GlAccount.Width = 75;
            // 
            // Duration
            // 
            this.Duration.HeaderText = "Duration";
            this.Duration.Name = "Duration";
            this.Duration.ReadOnly = true;
            this.Duration.Width = 50;
            // 
            // ConsultantFee
            // 
            this.ConsultantFee.HeaderText = "Consultant Fee";
            this.ConsultantFee.Name = "ConsultantFee";
            this.ConsultantFee.ReadOnly = true;
            this.ConsultantFee.Width = 110;
            // 
            // HospitalFee
            // 
            this.HospitalFee.HeaderText = "Hospital Fee";
            this.HospitalFee.Name = "HospitalFee";
            this.HospitalFee.ReadOnly = true;
            this.HospitalFee.Width = 90;
            // 
            // Total
            // 
            this.Total.HeaderText = "Total";
            this.Total.Name = "Total";
            this.Total.ReadOnly = true;
            // 
            // btnConsultant
            // 
            this.btnConsultant.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConsultant.BackgroundImage")));
            this.btnConsultant.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConsultant.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConsultant.Location = new System.Drawing.Point(403, 226);
            this.btnConsultant.Name = "btnConsultant";
            this.btnConsultant.Size = new System.Drawing.Size(32, 26);
            this.btnConsultant.TabIndex = 11;
            this.btnConsultant.UseVisualStyleBackColor = true;
            this.btnConsultant.Click += new System.EventHandler(this.btnConsultant_Click);
            // 
            // btnScanNames
            // 
            this.btnScanNames.Location = new System.Drawing.Point(406, 259);
            this.btnScanNames.Name = "btnScanNames";
            this.btnScanNames.Size = new System.Drawing.Size(32, 23);
            this.btnScanNames.TabIndex = 13;
            this.btnScanNames.Text = "...";
            this.btnScanNames.UseVisualStyleBackColor = true;
            this.btnScanNames.Click += new System.EventHandler(this.btnScanNames_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.MistyRose;
            this.label7.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(30, 441);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 14);
            this.label7.TabIndex = 49;
            this.label7.Text = "Payment Method";
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Enabled = false;
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPaymentMethod.Items.AddRange(new object[] {
            "Cash",
            "Cheque",
            "Credit Card",
            "Credit"});
            this.cmbPaymentMethod.Location = new System.Drawing.Point(178, 434);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(143, 22);
            this.cmbPaymentMethod.TabIndex = 15;
            // 
            // lblCreditCardNo
            // 
            this.lblCreditCardNo.AutoSize = true;
            this.lblCreditCardNo.BackColor = System.Drawing.Color.MistyRose;
            this.lblCreditCardNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreditCardNo.ForeColor = System.Drawing.Color.Black;
            this.lblCreditCardNo.Location = new System.Drawing.Point(30, 470);
            this.lblCreditCardNo.Name = "lblCreditCardNo";
            this.lblCreditCardNo.Size = new System.Drawing.Size(104, 14);
            this.lblCreditCardNo.TabIndex = 48;
            this.lblCreditCardNo.Text = "Credit Card No";
            // 
            // txtCreditCardNo
            // 
            this.txtCreditCardNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCreditCardNo.Location = new System.Drawing.Point(190, 470);
            this.txtCreditCardNo.Name = "txtCreditCardNo";
            this.txtCreditCardNo.ReadOnly = true;
            this.txtCreditCardNo.Size = new System.Drawing.Size(161, 22);
            this.txtCreditCardNo.TabIndex = 9;
            // 
            // dtpDueDate
            // 
            this.dtpDueDate.Enabled = false;
            this.dtpDueDate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDueDate.Location = new System.Drawing.Point(522, 71);
            this.dtpDueDate.Name = "dtpDueDate";
            this.dtpDueDate.Size = new System.Drawing.Size(98, 22);
            this.dtpDueDate.TabIndex = 3;
            // 
            // dtpDate
            // 
            this.dtpDate.Enabled = false;
            this.dtpDate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(160, 72);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(106, 22);
            this.dtpDate.TabIndex = 2;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.MistyRose;
            this.label13.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(24, 239);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(141, 14);
            this.label13.TabIndex = 43;
            this.label13.Text = "Institute/Technician";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Gainsboro;
            this.label5.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(22, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 29);
            this.label5.TabIndex = 44;
            this.label5.Text = "PLN";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.MistyRose;
            this.label8.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(363, 96);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 14);
            this.label8.TabIndex = 4;
            this.label8.Text = "Channeling NO";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.MistyRose;
            this.label2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(24, 272);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 14);
            this.label2.TabIndex = 41;
            this.label2.Text = "Xray Name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.BackColor = System.Drawing.Color.MistyRose;
            this.lblFirstName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirstName.ForeColor = System.Drawing.Color.Black;
            this.lblFirstName.Location = new System.Drawing.Point(24, 131);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(96, 14);
            this.lblFirstName.TabIndex = 42;
            this.lblFirstName.Text = "Patient Name";
            this.lblFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtReceiptNo
            // 
            this.txtReceiptNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReceiptNo.Location = new System.Drawing.Point(522, 45);
            this.txtReceiptNo.Name = "txtReceiptNo";
            this.txtReceiptNo.ReadOnly = true;
            this.txtReceiptNo.Size = new System.Drawing.Size(161, 22);
            this.txtReceiptNo.TabIndex = 1;
            // 
            // txtTokenNo
            // 
            this.txtTokenNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTokenNo.Location = new System.Drawing.Point(521, 99);
            this.txtTokenNo.Name = "txtTokenNo";
            this.txtTokenNo.ReadOnly = true;
            this.txtTokenNo.Size = new System.Drawing.Size(161, 22);
            this.txtTokenNo.TabIndex = 6;
            // 
            // txtScanName
            // 
            this.txtScanName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScanName.Location = new System.Drawing.Point(162, 259);
            this.txtScanName.Name = "txtScanName";
            this.txtScanName.ReadOnly = true;
            this.txtScanName.Size = new System.Drawing.Size(240, 22);
            this.txtScanName.TabIndex = 12;
            this.txtScanName.TextChanged += new System.EventHandler(this.txtScanName_TextChanged);
            // 
            // txtFirstName
            // 
            this.txtFirstName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(160, 124);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.ReadOnly = true;
            this.txtFirstName.Size = new System.Drawing.Size(357, 22);
            this.txtFirstName.TabIndex = 7;
            this.txtFirstName.TextChanged += new System.EventHandler(this.txtFirstName_TextChanged);
            this.txtFirstName.Leave += new System.EventHandler(this.txtFirstName_Leave);
            // 
            // lblContactNo
            // 
            this.lblContactNo.AutoSize = true;
            this.lblContactNo.BackColor = System.Drawing.Color.MistyRose;
            this.lblContactNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContactNo.ForeColor = System.Drawing.Color.Black;
            this.lblContactNo.Location = new System.Drawing.Point(24, 159);
            this.lblContactNo.Name = "lblContactNo";
            this.lblContactNo.Size = new System.Drawing.Size(79, 14);
            this.lblContactNo.TabIndex = 39;
            this.lblContactNo.Text = "Contact No";
            this.lblContactNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtConsultant
            // 
            this.txtConsultant.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsultant.Location = new System.Drawing.Point(161, 230);
            this.txtConsultant.Name = "txtConsultant";
            this.txtConsultant.ReadOnly = true;
            this.txtConsultant.Size = new System.Drawing.Size(240, 22);
            this.txtConsultant.TabIndex = 10;
            this.txtConsultant.TextChanged += new System.EventHandler(this.txtConsultant_TextChanged);
            // 
            // txtContactNo
            // 
            this.txtContactNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContactNo.Location = new System.Drawing.Point(160, 152);
            this.txtContactNo.Name = "txtContactNo";
            this.txtContactNo.ReadOnly = true;
            this.txtContactNo.Size = new System.Drawing.Size(131, 22);
            this.txtContactNo.TabIndex = 8;
            // 
            // lblRemarks
            // 
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.BackColor = System.Drawing.Color.MistyRose;
            this.lblRemarks.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemarks.ForeColor = System.Drawing.Color.Black;
            this.lblRemarks.Location = new System.Drawing.Point(24, 187);
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size(65, 14);
            this.lblRemarks.TabIndex = 38;
            this.lblRemarks.Text = "Remarks";
            this.lblRemarks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtRemarks
            // 
            this.txtRemarks.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRemarks.Location = new System.Drawing.Point(160, 178);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.ReadOnly = true;
            this.txtRemarks.Size = new System.Drawing.Size(357, 45);
            this.txtRemarks.TabIndex = 9;
            this.txtRemarks.TextChanged += new System.EventHandler(this.txtRemarks_TextChanged);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.Snow;
            this.btnNew.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.ForeColor = System.Drawing.Color.Black;
            this.btnNew.Location = new System.Drawing.Point(88, 15);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 33);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Snow;
            this.btnSave.Enabled = false;
            this.btnSave.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.Black;
            this.btnSave.Location = new System.Drawing.Point(250, 15);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 33);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save / Print";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.Snow;
            this.btnEdit.Enabled = false;
            this.btnEdit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.ForeColor = System.Drawing.Color.Black;
            this.btnEdit.Location = new System.Drawing.Point(169, 15);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 33);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Snow;
            this.btnDelete.Enabled = false;
            this.btnDelete.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.Black;
            this.btnDelete.Location = new System.Drawing.Point(617, 585);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(29, 19);
            this.btnDelete.TabIndex = 58;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Snow;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(379, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 33);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.MistyRose;
            this.label6.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(371, 446);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 14);
            this.label6.TabIndex = 61;
            this.label6.Text = "Receipt Total";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTotal
            // 
            this.txtTotal.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(512, 444);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(134, 22);
            this.txtTotal.TabIndex = 11;
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.MistyRose;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(212, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 14);
            this.label1.TabIndex = 63;
            this.label1.Text = "SearchBy";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.MistyRose;
            this.label10.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(461, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 14);
            this.label10.TabIndex = 62;
            this.label10.Text = "Search";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtConsultantFind
            // 
            this.txtConsultantFind.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsultantFind.Location = new System.Drawing.Point(533, 25);
            this.txtConsultantFind.Name = "txtConsultantFind";
            this.txtConsultantFind.Size = new System.Drawing.Size(161, 22);
            this.txtConsultantFind.TabIndex = 64;
            this.txtConsultantFind.TextChanged += new System.EventHandler(this.txtConsultantFind_TextChanged);
            // 
            // cmbScanSerch
            // 
            this.cmbScanSerch.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbScanSerch.FormattingEnabled = true;
            this.cmbScanSerch.Items.AddRange(new object[] {
            "ReceiptNo",
            "Patient Name",
            "ContactNo"});
            this.cmbScanSerch.Location = new System.Drawing.Point(301, 25);
            this.cmbScanSerch.Name = "cmbScanSerch";
            this.cmbScanSerch.Size = new System.Drawing.Size(146, 22);
            this.cmbScanSerch.TabIndex = 65;
            // 
            // btnReprintXray
            // 
            this.btnReprintXray.BackColor = System.Drawing.Color.Snow;
            this.btnReprintXray.Enabled = false;
            this.btnReprintXray.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReprintXray.ForeColor = System.Drawing.Color.Black;
            this.btnReprintXray.Location = new System.Drawing.Point(7, 15);
            this.btnReprintXray.Name = "btnReprintXray";
            this.btnReprintXray.Size = new System.Drawing.Size(75, 33);
            this.btnReprintXray.TabIndex = 4;
            this.btnReprintXray.Text = "Reprint";
            this.btnReprintXray.UseVisualStyleBackColor = false;
            this.btnReprintXray.Click += new System.EventHandler(this.btnReprintXray_Click);
            // 
            // Clear
            // 
            this.Clear.BackColor = System.Drawing.Color.Snow;
            this.Clear.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Clear.ForeColor = System.Drawing.Color.Black;
            this.Clear.Location = new System.Drawing.Point(630, 253);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(62, 29);
            this.Clear.TabIndex = 66;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = false;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.MistyRose;
            this.label3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(24, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 14);
            this.label3.TabIndex = 40;
            this.label3.Text = "Appointment Date";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.MistyRose;
            this.label4.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(364, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 14);
            this.label4.TabIndex = 1;
            this.label4.Text = "Collect Date";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.MistyRose;
            this.label9.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(363, 46);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 14);
            this.label9.TabIndex = 40;
            this.label9.Text = "Receipt NO";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.MistyRose;
            this.label11.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(22, 57);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 14);
            this.label11.TabIndex = 40;
            this.label11.Text = "Receipt Date";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpRepDate
            // 
            this.dtpRepDate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpRepDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRepDate.Location = new System.Drawing.Point(160, 46);
            this.dtpRepDate.Name = "dtpRepDate";
            this.dtpRepDate.Size = new System.Drawing.Size(106, 22);
            this.dtpRepDate.TabIndex = 0;
            // 
            // btnRefund
            // 
            this.btnRefund.BackColor = System.Drawing.Color.Snow;
            this.btnRefund.Enabled = false;
            this.btnRefund.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefund.ForeColor = System.Drawing.Color.Black;
            this.btnRefund.Location = new System.Drawing.Point(130, 561);
            this.btnRefund.Name = "btnRefund";
            this.btnRefund.Size = new System.Drawing.Size(75, 33);
            this.btnRefund.TabIndex = 18;
            this.btnRefund.Text = "Refund";
            this.btnRefund.UseVisualStyleBackColor = false;
            this.btnRefund.Click += new System.EventHandler(this.btnRefund_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.MistyRose;
            this.label12.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(371, 503);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 14);
            this.label12.TabIndex = 69;
            this.label12.Text = "Net Total";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.MistyRose;
            this.label14.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(31, 499);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(111, 14);
            this.label14.TabIndex = 68;
            this.label14.Text = "Disount Amount";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.MistyRose;
            this.label15.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(371, 475);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(98, 14);
            this.label15.TabIndex = 67;
            this.label15.Text = "Discount Rate";
            // 
            // txtnetTotal
            // 
            this.txtnetTotal.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtnetTotal.Location = new System.Drawing.Point(512, 499);
            this.txtnetTotal.Name = "txtnetTotal";
            this.txtnetTotal.ReadOnly = true;
            this.txtnetTotal.Size = new System.Drawing.Size(155, 22);
            this.txtnetTotal.TabIndex = 13;
            this.txtnetTotal.Text = "0";
            // 
            // txtDisAmount
            // 
            this.txtDisAmount.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDisAmount.Location = new System.Drawing.Point(190, 498);
            this.txtDisAmount.Name = "txtDisAmount";
            this.txtDisAmount.ReadOnly = true;
            this.txtDisAmount.Size = new System.Drawing.Size(162, 22);
            this.txtDisAmount.TabIndex = 10;
            this.txtDisAmount.Text = "0";
            // 
            // txtdisRate
            // 
            this.txtdisRate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtdisRate.Location = new System.Drawing.Point(500, 463);
            this.txtdisRate.Name = "txtdisRate";
            this.txtdisRate.ReadOnly = true;
            this.txtdisRate.Size = new System.Drawing.Size(70, 22);
            this.txtdisRate.TabIndex = 16;
            this.txtdisRate.Text = "0";
            this.txtdisRate.TextChanged += new System.EventHandler(this.txtdisRate_TextChanged);
            // 
            // rbtnboth
            // 
            this.rbtnboth.AutoSize = true;
            this.rbtnboth.BackColor = System.Drawing.Color.MistyRose;
            this.rbtnboth.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnboth.Location = new System.Drawing.Point(16, 586);
            this.rbtnboth.Name = "rbtnboth";
            this.rbtnboth.Size = new System.Drawing.Size(55, 18);
            this.rbtnboth.TabIndex = 17;
            this.rbtnboth.TabStop = true;
            this.rbtnboth.Text = "Both";
            this.rbtnboth.UseVisualStyleBackColor = false;
            this.rbtnboth.CheckedChanged += new System.EventHandler(this.rbtnboth_CheckedChanged);
            // 
            // rbtnDoctorfee
            // 
            this.rbtnDoctorfee.AutoSize = true;
            this.rbtnDoctorfee.BackColor = System.Drawing.Color.MistyRose;
            this.rbtnDoctorfee.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnDoctorfee.Location = new System.Drawing.Point(17, 548);
            this.rbtnDoctorfee.Name = "rbtnDoctorfee";
            this.rbtnDoctorfee.Size = new System.Drawing.Size(97, 18);
            this.rbtnDoctorfee.TabIndex = 15;
            this.rbtnDoctorfee.TabStop = true;
            this.rbtnDoctorfee.Text = "Doctor Fee";
            this.rbtnDoctorfee.UseVisualStyleBackColor = false;
            this.rbtnDoctorfee.CheckedChanged += new System.EventHandler(this.rbtnDoctorfee_CheckedChanged);
            // 
            // rbtnHosfee
            // 
            this.rbtnHosfee.AutoSize = true;
            this.rbtnHosfee.BackColor = System.Drawing.Color.MistyRose;
            this.rbtnHosfee.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnHosfee.Location = new System.Drawing.Point(17, 566);
            this.rbtnHosfee.Name = "rbtnHosfee";
            this.rbtnHosfee.Size = new System.Drawing.Size(107, 18);
            this.rbtnHosfee.TabIndex = 16;
            this.rbtnHosfee.TabStop = true;
            this.rbtnHosfee.Text = "Hospital Fee";
            this.rbtnHosfee.UseVisualStyleBackColor = false;
            this.rbtnHosfee.CheckedChanged += new System.EventHandler(this.rbtnHosfee_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.MistyRose;
            this.groupBox1.Controls.Add(this.rbtncash);
            this.groupBox1.Controls.Add(this.rbtnbht);
            this.groupBox1.Controls.Add(this.rbtnetuopd);
            this.groupBox1.Controls.Add(this.btnScanNames);
            this.groupBox1.Controls.Add(this.btnConsultant);
            this.groupBox1.Controls.Add(this.dgvScanItems);
            this.groupBox1.Controls.Add(this.txtdisRate);
            this.groupBox1.Controls.Add(this.cmbPaymentMethod);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.dtpRepDate);
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.txtRemarks);
            this.groupBox1.Controls.Add(this.txtContactNo);
            this.groupBox1.Controls.Add(this.txtFirstName);
            this.groupBox1.Controls.Add(this.txtConsultant);
            this.groupBox1.Controls.Add(this.txtTokenNo);
            this.groupBox1.Controls.Add(this.dtpDueDate);
            this.groupBox1.Controls.Add(this.txtReceiptNo);
            this.groupBox1.Controls.Add(this.txtPatientNo);
            this.groupBox1.Controls.Add(this.cbxInpatient);
            this.groupBox1.Controls.Add(this.txtScanTotal);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.dgvScanRefundSave);
            this.groupBox1.Controls.Add(this.Clear);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtScanName);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(717, 521);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // rbtncash
            // 
            this.rbtncash.AutoSize = true;
            this.rbtncash.Enabled = false;
            this.rbtncash.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtncash.Location = new System.Drawing.Point(300, 59);
            this.rbtncash.Name = "rbtncash";
            this.rbtncash.Size = new System.Drawing.Size(58, 17);
            this.rbtncash.TabIndex = 148;
            this.rbtncash.TabStop = true;
            this.rbtncash.Text = "CASH";
            this.rbtncash.UseVisualStyleBackColor = true;
            this.rbtncash.CheckedChanged += new System.EventHandler(this.rbtncash_CheckedChanged);
            // 
            // rbtnbht
            // 
            this.rbtnbht.AutoSize = true;
            this.rbtnbht.Enabled = false;
            this.rbtnbht.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnbht.Location = new System.Drawing.Point(300, 99);
            this.rbtnbht.Name = "rbtnbht";
            this.rbtnbht.Size = new System.Drawing.Size(41, 17);
            this.rbtnbht.TabIndex = 149;
            this.rbtnbht.TabStop = true;
            this.rbtnbht.Text = "IUI";
            this.rbtnbht.UseVisualStyleBackColor = true;
            this.rbtnbht.CheckedChanged += new System.EventHandler(this.rbtnbht_CheckedChanged);
            // 
            // rbtnetuopd
            // 
            this.rbtnetuopd.AutoSize = true;
            this.rbtnetuopd.Enabled = false;
            this.rbtnetuopd.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnetuopd.Location = new System.Drawing.Point(300, 78);
            this.rbtnetuopd.Name = "rbtnetuopd";
            this.rbtnetuopd.Size = new System.Drawing.Size(73, 17);
            this.rbtnetuopd.TabIndex = 147;
            this.rbtnetuopd.TabStop = true;
            this.rbtnetuopd.Text = "ETU/OPD";
            this.rbtnetuopd.UseVisualStyleBackColor = true;
            this.rbtnetuopd.CheckedChanged += new System.EventHandler(this.rbtnetuopd_CheckedChanged);
            // 
            // txtPatientNo
            // 
            this.txtPatientNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPatientNo.Location = new System.Drawing.Point(161, 96);
            this.txtPatientNo.Name = "txtPatientNo";
            this.txtPatientNo.ReadOnly = true;
            this.txtPatientNo.Size = new System.Drawing.Size(130, 22);
            this.txtPatientNo.TabIndex = 4;
            this.txtPatientNo.TextChanged += new System.EventHandler(this.txtPatientNo_TextChanged);
            // 
            // cbxInpatient
            // 
            this.cbxInpatient.AutoSize = true;
            this.cbxInpatient.Enabled = false;
            this.cbxInpatient.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxInpatient.Location = new System.Drawing.Point(561, 214);
            this.cbxInpatient.Name = "cbxInpatient";
            this.cbxInpatient.Size = new System.Drawing.Size(94, 20);
            this.cbxInpatient.TabIndex = 5;
            this.cbxInpatient.Text = "Inpatient";
            this.cbxInpatient.UseVisualStyleBackColor = true;
            this.cbxInpatient.Visible = false;
            // 
            // txtScanTotal
            // 
            this.txtScanTotal.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScanTotal.Location = new System.Drawing.Point(640, 435);
            this.txtScanTotal.Name = "txtScanTotal";
            this.txtScanTotal.ReadOnly = true;
            this.txtScanTotal.Size = new System.Drawing.Size(32, 22);
            this.txtScanTotal.TabIndex = 8;
            this.txtScanTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtScanTotal.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.MistyRose;
            this.label16.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.Black;
            this.label16.Location = new System.Drawing.Point(14, 96);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(76, 14);
            this.label16.TabIndex = 69;
            this.label16.Text = "Patient No";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvScanRefundSave
            // 
            this.dgvScanRefundSave.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScanRefundSave.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7});
            this.dgvScanRefundSave.Location = new System.Drawing.Point(587, 143);
            this.dgvScanRefundSave.Name = "dgvScanRefundSave";
            this.dgvScanRefundSave.ReadOnly = true;
            this.dgvScanRefundSave.RowHeadersVisible = false;
            this.dgvScanRefundSave.Size = new System.Drawing.Size(95, 49);
            this.dgvScanRefundSave.TabIndex = 67;
            this.dgvScanRefundSave.Visible = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "ScanID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "ScanName";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 150;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle1.NullValue = null;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn3.HeaderText = "Account";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 75;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Duration";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 50;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn5.HeaderText = "Consultant Fee";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 110;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn6.HeaderText = "ScanningFee";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 90;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn7.HeaderText = "Total";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.MistyRose;
            this.groupBox2.Controls.Add(this.btnEdit);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.btnNew);
            this.groupBox2.Controls.Add(this.btnReprintXray);
            this.groupBox2.Location = new System.Drawing.Point(249, 536);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 74);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.MistyRose;
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 536);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(220, 74);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Refund";
            // 
            // frmXray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(741, 622);
            this.Controls.Add(this.rbtnboth);
            this.Controls.Add(this.rbtnDoctorfee);
            this.Controls.Add(this.rbtnHosfee);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtnetTotal);
            this.Controls.Add(this.txtDisAmount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtConsultantFind);
            this.Controls.Add(this.cmbScanSerch);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.btnRefund);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblCreditCardNo);
            this.Controls.Add(this.txtCreditCardNo);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.lblContactNo);
            this.Controls.Add(this.lblRemarks);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Location = new System.Drawing.Point(10, 80);
            this.Name = "frmXray";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmXray";
            this.Load += new System.EventHandler(this.frmXray_Load);
            this.Activated += new System.EventHandler(this.frmXray_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScanItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScanRefundSave)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvScanItems;
        private System.Windows.Forms.Button btnConsultant;
        private System.Windows.Forms.Button btnScanNames;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label lblCreditCardNo;
        private System.Windows.Forms.TextBox txtCreditCardNo;
        private System.Windows.Forms.DateTimePicker dtpDueDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.TextBox txtReceiptNo;
        private System.Windows.Forms.TextBox txtTokenNo;
        private System.Windows.Forms.TextBox txtScanName;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label lblContactNo;
        private System.Windows.Forms.TextBox txtConsultant;
        private System.Windows.Forms.TextBox txtContactNo;
        private System.Windows.Forms.Label lblRemarks;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn XrayID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScanName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GlAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Duration;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConsultantFee;
        private System.Windows.Forms.DataGridViewTextBoxColumn HospitalFee;
        private System.Windows.Forms.DataGridViewTextBoxColumn Total;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtConsultantFind;
        private System.Windows.Forms.ComboBox cmbScanSerch;
        private System.Windows.Forms.Button btnReprintXray;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpRepDate;
        private System.Windows.Forms.Button btnRefund;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtnetTotal;
        private System.Windows.Forms.TextBox txtDisAmount;
        private System.Windows.Forms.TextBox txtdisRate;
        private System.Windows.Forms.RadioButton rbtnboth;
        private System.Windows.Forms.RadioButton rbtnDoctorfee;
        private System.Windows.Forms.RadioButton rbtnHosfee;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dgvScanRefundSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtPatientNo;
        private System.Windows.Forms.TextBox txtScanTotal;
        private System.Windows.Forms.CheckBox cbxInpatient;
        private System.Windows.Forms.RadioButton rbtncash;
        private System.Windows.Forms.RadioButton rbtnbht;
        private System.Windows.Forms.RadioButton rbtnetuopd;
    }
}
using System.Data;

namespace UserAutherization
{
    partial class frmChannellig
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChannellig));
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtContactNo = new System.Windows.Forms.TextBox();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.lblContactNo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbNormalBill = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.UltrInsuranceCampany = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.txtconsultIntime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSession = new System.Windows.Forms.ComboBox();
            this.lblSession1 = new System.Windows.Forms.Label();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.txtTestType = new System.Windows.Forms.TextBox();
            this.txtPationType = new System.Windows.Forms.TextBox();
            this.rbcash = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtnetTotal = new System.Windows.Forms.TextBox();
            this.txtDisAmount = new System.Windows.Forms.TextBox();
            this.txtdisRate = new System.Windows.Forms.TextBox();
            this.Clear = new System.Windows.Forms.Button();
            this.dgvScanItems = new System.Windows.Forms.DataGridView();
            this.Scanid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScanName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GlAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConsultantFee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HospitalFee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnConsultant = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.lblCreditCardNo = new System.Windows.Forms.Label();
            this.txtCreditCardNo = new System.Windows.Forms.TextBox();
            this.dtpRepDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtReceiptNo = new System.Windows.Forms.TextBox();
            this.txtTokenNo = new System.Windows.Forms.TextBox();
            this.txtConsultant = new System.Windows.Forms.TextBox();
            this.txtPatientNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.btnNewn = new System.Windows.Forms.ToolStripButton();
            this.btnList = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnConfirm = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.btnVoid = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltrInsuranceCampany)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScanItems)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFirstName
            // 
            this.txtFirstName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(160, 146);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.ReadOnly = true;
            this.txtFirstName.Size = new System.Drawing.Size(334, 22);
            this.txtFirstName.TabIndex = 4;
            this.txtFirstName.TextChanged += new System.EventHandler(this.txtFirstName_TextChanged);
            this.txtFirstName.Leave += new System.EventHandler(this.txtFirstName_Leave);
            // 
            // txtContactNo
            // 
            this.txtContactNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContactNo.Location = new System.Drawing.Point(159, 96);
            this.txtContactNo.Name = "txtContactNo";
            this.txtContactNo.ReadOnly = true;
            this.txtContactNo.Size = new System.Drawing.Size(138, 22);
            this.txtContactNo.TabIndex = 3;
            this.txtContactNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContactNo_KeyPress);
            this.txtContactNo.Leave += new System.EventHandler(this.txtContactNo_Leave);
            // 
            // txtRemarks
            // 
            this.txtRemarks.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRemarks.Location = new System.Drawing.Point(160, 173);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.ReadOnly = true;
            this.txtRemarks.Size = new System.Drawing.Size(357, 45);
            this.txtRemarks.TabIndex = 5;
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirstName.ForeColor = System.Drawing.Color.Black;
            this.lblFirstName.Location = new System.Drawing.Point(16, 145);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(96, 16);
            this.lblFirstName.TabIndex = 19;
            this.lblFirstName.Text = "Patient Name";
            this.lblFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFirstName.Click += new System.EventHandler(this.lblFirstName_Click);
            // 
            // lblRemarks
            // 
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemarks.ForeColor = System.Drawing.Color.Black;
            this.lblRemarks.Location = new System.Drawing.Point(16, 177);
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size(62, 16);
            this.lblRemarks.TabIndex = 17;
            this.lblRemarks.Text = "Remarks";
            this.lblRemarks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRemarks.Click += new System.EventHandler(this.lblRemarks_Click);
            // 
            // lblContactNo
            // 
            this.lblContactNo.AutoSize = true;
            this.lblContactNo.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContactNo.ForeColor = System.Drawing.Color.Black;
            this.lblContactNo.Location = new System.Drawing.Point(15, 96);
            this.lblContactNo.Name = "lblContactNo";
            this.lblContactNo.Size = new System.Drawing.Size(83, 16);
            this.lblContactNo.TabIndex = 18;
            this.lblContactNo.Text = "Contact No";
            this.lblContactNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblContactNo.Click += new System.EventHandler(this.lblContactNo_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.cbNormalBill);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.UltrInsuranceCampany);
            this.groupBox1.Controls.Add(this.txtconsultIntime);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbSession);
            this.groupBox1.Controls.Add(this.lblSession1);
            this.groupBox1.Controls.Add(this.dtpTime);
            this.groupBox1.Controls.Add(this.txtTestType);
            this.groupBox1.Controls.Add(this.txtPationType);
            this.groupBox1.Controls.Add(this.rbcash);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txtnetTotal);
            this.groupBox1.Controls.Add(this.txtDisAmount);
            this.groupBox1.Controls.Add(this.txtdisRate);
            this.groupBox1.Controls.Add(this.Clear);
            this.groupBox1.Controls.Add(this.dgvScanItems);
            this.groupBox1.Controls.Add(this.btnConsultant);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cmbPaymentMethod);
            this.groupBox1.Controls.Add(this.lblCreditCardNo);
            this.groupBox1.Controls.Add(this.txtCreditCardNo);
            this.groupBox1.Controls.Add(this.dtpRepDate);
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtTotal);
            this.groupBox1.Controls.Add(this.lblFirstName);
            this.groupBox1.Controls.Add(this.txtReceiptNo);
            this.groupBox1.Controls.Add(this.txtTokenNo);
            this.groupBox1.Controls.Add(this.txtFirstName);
            this.groupBox1.Controls.Add(this.lblContactNo);
            this.groupBox1.Controls.Add(this.txtConsultant);
            this.groupBox1.Controls.Add(this.txtPatientNo);
            this.groupBox1.Controls.Add(this.txtContactNo);
            this.groupBox1.Controls.Add(this.lblRemarks);
            this.groupBox1.Controls.Add(this.txtRemarks);
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(8, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(713, 469);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // cbNormalBill
            // 
            this.cbNormalBill.AutoSize = true;
            this.cbNormalBill.Enabled = false;
            this.cbNormalBill.Font = new System.Drawing.Font("Verdana", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbNormalBill.Location = new System.Drawing.Point(527, 224);
            this.cbNormalBill.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbNormalBill.Name = "cbNormalBill";
            this.cbNormalBill.Size = new System.Drawing.Size(100, 21);
            this.cbNormalBill.TabIndex = 230;
            this.cbNormalBill.Text = "Normal Bill";
            this.cbNormalBill.UseVisualStyleBackColor = true;
            this.cbNormalBill.CheckedChanged += new System.EventHandler(this.cbNormalBill_CheckedChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.Black;
            this.label20.Location = new System.Drawing.Point(16, 226);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(138, 16);
            this.label20.TabIndex = 229;
            this.label20.Text = "Insurence Company";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UltrInsuranceCampany
            // 
            this.UltrInsuranceCampany.CheckedListSettings.CheckStateMember = "";
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.UltrInsuranceCampany.DisplayLayout.Appearance = appearance1;
            this.UltrInsuranceCampany.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.UltrInsuranceCampany.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.UltrInsuranceCampany.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.UltrInsuranceCampany.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.UltrInsuranceCampany.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.UltrInsuranceCampany.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.UltrInsuranceCampany.DisplayLayout.MaxColScrollRegions = 1;
            this.UltrInsuranceCampany.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.UltrInsuranceCampany.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.UltrInsuranceCampany.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.UltrInsuranceCampany.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.UltrInsuranceCampany.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.UltrInsuranceCampany.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.UltrInsuranceCampany.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.UltrInsuranceCampany.DisplayLayout.Override.CellAppearance = appearance8;
            this.UltrInsuranceCampany.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.UltrInsuranceCampany.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.UltrInsuranceCampany.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.UltrInsuranceCampany.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.UltrInsuranceCampany.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.UltrInsuranceCampany.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.UltrInsuranceCampany.DisplayLayout.Override.RowAppearance = appearance11;
            this.UltrInsuranceCampany.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance25.BackColor = System.Drawing.SystemColors.ControlLight;
            this.UltrInsuranceCampany.DisplayLayout.Override.TemplateAddRowAppearance = appearance25;
            this.UltrInsuranceCampany.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.UltrInsuranceCampany.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.UltrInsuranceCampany.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.UltrInsuranceCampany.Enabled = false;
            this.UltrInsuranceCampany.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UltrInsuranceCampany.Location = new System.Drawing.Point(160, 222);
            this.UltrInsuranceCampany.Name = "UltrInsuranceCampany";
            this.UltrInsuranceCampany.Size = new System.Drawing.Size(357, 25);
            this.UltrInsuranceCampany.TabIndex = 228;
            // 
            // txtconsultIntime
            // 
            this.txtconsultIntime.Enabled = false;
            this.txtconsultIntime.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtconsultIntime.Location = new System.Drawing.Point(489, 280);
            this.txtconsultIntime.Name = "txtconsultIntime";
            this.txtconsultIntime.Size = new System.Drawing.Size(126, 22);
            this.txtconsultIntime.TabIndex = 227;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(340, 283);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 226;
            this.label1.Text = "Consultant InTime";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbSession
            // 
            this.cmbSession.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSession.Enabled = false;
            this.cmbSession.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSession.Items.AddRange(new object[] {
            "Session 1",
            "Session 2",
            "Session 3"});
            this.cmbSession.Location = new System.Drawing.Point(160, 277);
            this.cmbSession.Name = "cmbSession";
            this.cmbSession.Size = new System.Drawing.Size(161, 24);
            this.cmbSession.TabIndex = 225;
            this.cmbSession.SelectedIndexChanged += new System.EventHandler(this.cmbSession_SelectedIndexChanged);
            // 
            // lblSession1
            // 
            this.lblSession1.AutoSize = true;
            this.lblSession1.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.lblSession1.Location = new System.Drawing.Point(18, 280);
            this.lblSession1.Name = "lblSession1";
            this.lblSession1.Size = new System.Drawing.Size(58, 16);
            this.lblSession1.TabIndex = 224;
            this.lblSession1.Text = "Session";
            // 
            // dtpTime
            // 
            this.dtpTime.Enabled = false;
            this.dtpTime.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTime.Location = new System.Drawing.Point(276, 42);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.Size = new System.Drawing.Size(103, 22);
            this.dtpTime.TabIndex = 159;
            // 
            // txtTestType
            // 
            this.txtTestType.Enabled = false;
            this.txtTestType.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTestType.Location = new System.Drawing.Point(159, 14);
            this.txtTestType.Name = "txtTestType";
            this.txtTestType.ReadOnly = true;
            this.txtTestType.Size = new System.Drawing.Size(107, 22);
            this.txtTestType.TabIndex = 158;
            this.txtTestType.Text = "CHANNELLING";
            // 
            // txtPationType
            // 
            this.txtPationType.Enabled = false;
            this.txtPationType.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPationType.Location = new System.Drawing.Point(527, 16);
            this.txtPationType.Name = "txtPationType";
            this.txtPationType.Size = new System.Drawing.Size(161, 22);
            this.txtPationType.TabIndex = 8;
            // 
            // rbcash
            // 
            this.rbcash.AutoSize = true;
            this.rbcash.Enabled = false;
            this.rbcash.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbcash.Location = new System.Drawing.Point(307, 123);
            this.rbcash.Name = "rbcash";
            this.rbcash.Size = new System.Drawing.Size(55, 17);
            this.rbcash.TabIndex = 2;
            this.rbcash.Text = "Cash";
            this.rbcash.UseVisualStyleBackColor = true;
            this.rbcash.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.Black;
            this.label18.Location = new System.Drawing.Point(419, 16);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(92, 16);
            this.label18.TabIndex = 149;
            this.label18.Text = "Patient Type";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(16, 19);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(74, 16);
            this.label17.TabIndex = 147;
            this.label17.Text = "Test Type";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(364, 432);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 16);
            this.label9.TabIndex = 34;
            this.label9.Text = "Net Total";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(22, 433);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 16);
            this.label11.TabIndex = 33;
            this.label11.Text = "Disount Amount";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(364, 406);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 16);
            this.label15.TabIndex = 32;
            this.label15.Text = "Discount Rate";
            // 
            // txtnetTotal
            // 
            this.txtnetTotal.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtnetTotal.Location = new System.Drawing.Point(505, 428);
            this.txtnetTotal.Name = "txtnetTotal";
            this.txtnetTotal.ReadOnly = true;
            this.txtnetTotal.Size = new System.Drawing.Size(155, 22);
            this.txtnetTotal.TabIndex = 19;
            this.txtnetTotal.Text = "0";
            // 
            // txtDisAmount
            // 
            this.txtDisAmount.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDisAmount.Location = new System.Drawing.Point(181, 432);
            this.txtDisAmount.Name = "txtDisAmount";
            this.txtDisAmount.ReadOnly = true;
            this.txtDisAmount.Size = new System.Drawing.Size(162, 22);
            this.txtDisAmount.TabIndex = 16;
            this.txtDisAmount.Text = "0";
            // 
            // txtdisRate
            // 
            this.txtdisRate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtdisRate.Location = new System.Drawing.Point(505, 404);
            this.txtdisRate.Name = "txtdisRate";
            this.txtdisRate.ReadOnly = true;
            this.txtdisRate.Size = new System.Drawing.Size(70, 22);
            this.txtdisRate.TabIndex = 10;
            this.txtdisRate.Text = "0";
            this.txtdisRate.TextChanged += new System.EventHandler(this.txtdisRate_TextChanged);
            // 
            // Clear
            // 
            this.Clear.BackColor = System.Drawing.Color.Snow;
            this.Clear.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Clear.ForeColor = System.Drawing.Color.Black;
            this.Clear.Location = new System.Drawing.Point(621, 277);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(75, 33);
            this.Clear.TabIndex = 13;
            this.Clear.Text = "CLEAR";
            this.Clear.UseVisualStyleBackColor = false;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // dgvScanItems
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvScanItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvScanItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvScanItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScanItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Scanid,
            this.ScanName,
            this.GlAccount,
            this.Duration,
            this.ConsultantFee,
            this.HospitalFee,
            this.Total});
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvScanItems.DefaultCellStyle = dataGridViewCellStyle10;
            this.dgvScanItems.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvScanItems.Location = new System.Drawing.Point(17, 314);
            this.dgvScanItems.Name = "dgvScanItems";
            this.dgvScanItems.RowHeadersVisible = false;
            this.dgvScanItems.Size = new System.Drawing.Size(679, 53);
            this.dgvScanItems.TabIndex = 14;
            this.dgvScanItems.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvScanItems_CellClick);
            this.dgvScanItems.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvScanItems_CellEndEdit);
            this.dgvScanItems.Enter += new System.EventHandler(this.dgvScanItems_Enter);
            this.dgvScanItems.Leave += new System.EventHandler(this.dgvScanItems_Leave);
            // 
            // Scanid
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Scanid.DefaultCellStyle = dataGridViewCellStyle3;
            this.Scanid.HeaderText = "Item ID";
            this.Scanid.Name = "Scanid";
            this.Scanid.ReadOnly = true;
            // 
            // ScanName
            // 
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScanName.DefaultCellStyle = dataGridViewCellStyle4;
            this.ScanName.HeaderText = "Consultant Name";
            this.ScanName.Name = "ScanName";
            this.ScanName.ReadOnly = true;
            this.ScanName.Width = 200;
            // 
            // GlAccount
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.NullValue = null;
            this.GlAccount.DefaultCellStyle = dataGridViewCellStyle5;
            this.GlAccount.HeaderText = "Account";
            this.GlAccount.Name = "GlAccount";
            this.GlAccount.Visible = false;
            this.GlAccount.Width = 75;
            // 
            // Duration
            // 
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Duration.DefaultCellStyle = dataGridViewCellStyle6;
            this.Duration.HeaderText = "Collect On";
            this.Duration.Name = "Duration";
            this.Duration.Visible = false;
            this.Duration.Width = 110;
            // 
            // ConsultantFee
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConsultantFee.DefaultCellStyle = dataGridViewCellStyle7;
            this.ConsultantFee.HeaderText = "Consultant Fee";
            this.ConsultantFee.Name = "ConsultantFee";
            this.ConsultantFee.Width = 130;
            // 
            // HospitalFee
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HospitalFee.DefaultCellStyle = dataGridViewCellStyle8;
            this.HospitalFee.HeaderText = "Hospital Fee";
            this.HospitalFee.Name = "HospitalFee";
            this.HospitalFee.Width = 130;
            // 
            // Total
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Total.DefaultCellStyle = dataGridViewCellStyle9;
            this.Total.HeaderText = "Line Total";
            this.Total.Name = "Total";
            this.Total.Width = 110;
            // 
            // btnConsultant
            // 
            this.btnConsultant.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConsultant.BackgroundImage")));
            this.btnConsultant.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConsultant.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConsultant.Location = new System.Drawing.Point(402, 248);
            this.btnConsultant.Name = "btnConsultant";
            this.btnConsultant.Size = new System.Drawing.Size(33, 26);
            this.btnConsultant.TabIndex = 7;
            this.btnConsultant.UseVisualStyleBackColor = true;
            this.btnConsultant.Click += new System.EventHandler(this.btnConsultant_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(22, 383);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 16);
            this.label7.TabIndex = 23;
            this.label7.Text = "Payment Method";
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Enabled = false;
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPaymentMethod.Location = new System.Drawing.Point(182, 379);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(106, 24);
            this.cmbPaymentMethod.TabIndex = 9;
            this.cmbPaymentMethod.SelectedIndexChanged += new System.EventHandler(this.cmbPaymentMethod_SelectedIndexChanged);
            // 
            // lblCreditCardNo
            // 
            this.lblCreditCardNo.AutoSize = true;
            this.lblCreditCardNo.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreditCardNo.ForeColor = System.Drawing.Color.Black;
            this.lblCreditCardNo.Location = new System.Drawing.Point(22, 409);
            this.lblCreditCardNo.Name = "lblCreditCardNo";
            this.lblCreditCardNo.Size = new System.Drawing.Size(104, 16);
            this.lblCreditCardNo.TabIndex = 22;
            this.lblCreditCardNo.Text = "Credit Card No";
            // 
            // txtCreditCardNo
            // 
            this.txtCreditCardNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCreditCardNo.Location = new System.Drawing.Point(182, 405);
            this.txtCreditCardNo.Name = "txtCreditCardNo";
            this.txtCreditCardNo.ReadOnly = true;
            this.txtCreditCardNo.Size = new System.Drawing.Size(161, 22);
            this.txtCreditCardNo.TabIndex = 9;
            // 
            // dtpRepDate
            // 
            this.dtpRepDate.Enabled = false;
            this.dtpRepDate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpRepDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRepDate.Location = new System.Drawing.Point(160, 42);
            this.dtpRepDate.Name = "dtpRepDate";
            this.dtpRepDate.Size = new System.Drawing.Size(104, 22);
            this.dtpRepDate.TabIndex = 1;
            // 
            // dtpDate
            // 
            this.dtpDate.Enabled = false;
            this.dtpDate.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(159, 68);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(106, 22);
            this.dtpDate.TabIndex = 2;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(363, 382);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 16);
            this.label6.TabIndex = 19;
            this.label6.Text = "Gross Total";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.Enter += new System.EventHandler(this.label6_Enter);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(16, 250);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(79, 16);
            this.label13.TabIndex = 19;
            this.label13.Text = "Consultant";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(16, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "Receipt Date";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Click += new System.EventHandler(this.label4_Click_1);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.Black;
            this.label16.Location = new System.Drawing.Point(17, 122);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(77, 16);
            this.label16.TabIndex = 19;
            this.label16.Text = "Patient No";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label16.Click += new System.EventHandler(this.label16_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(16, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 16);
            this.label5.TabIndex = 19;
            this.label5.Text = "Appointment Date";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(419, 46);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 16);
            this.label12.TabIndex = 19;
            this.label12.Text = "Receipt No";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(419, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 16);
            this.label8.TabIndex = 19;
            this.label8.Text = "Channeling No";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // txtTotal
            // 
            this.txtTotal.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(505, 380);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(134, 22);
            this.txtTotal.TabIndex = 17;
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTotal.TextChanged += new System.EventHandler(this.txtTotal_TextChanged);
            // 
            // txtReceiptNo
            // 
            this.txtReceiptNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReceiptNo.Location = new System.Drawing.Point(527, 43);
            this.txtReceiptNo.Name = "txtReceiptNo";
            this.txtReceiptNo.ReadOnly = true;
            this.txtReceiptNo.Size = new System.Drawing.Size(161, 22);
            this.txtReceiptNo.TabIndex = 1;
            // 
            // txtTokenNo
            // 
            this.txtTokenNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTokenNo.Location = new System.Drawing.Point(527, 72);
            this.txtTokenNo.Name = "txtTokenNo";
            this.txtTokenNo.ReadOnly = true;
            this.txtTokenNo.Size = new System.Drawing.Size(161, 22);
            this.txtTokenNo.TabIndex = 6;
            this.txtTokenNo.TextChanged += new System.EventHandler(this.txtTokenNo_TextChanged);
            // 
            // txtConsultant
            // 
            this.txtConsultant.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsultant.Location = new System.Drawing.Point(160, 251);
            this.txtConsultant.Name = "txtConsultant";
            this.txtConsultant.ReadOnly = true;
            this.txtConsultant.Size = new System.Drawing.Size(240, 22);
            this.txtConsultant.TabIndex = 6;
            this.txtConsultant.TextChanged += new System.EventHandler(this.txtConsultant_TextChanged);
            // 
            // txtPatientNo
            // 
            this.txtPatientNo.Enabled = false;
            this.txtPatientNo.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPatientNo.Location = new System.Drawing.Point(159, 121);
            this.txtPatientNo.Name = "txtPatientNo";
            this.txtPatientNo.ReadOnly = true;
            this.txtPatientNo.Size = new System.Drawing.Size(139, 22);
            this.txtPatientNo.TabIndex = 1;
            this.txtPatientNo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtPatientNo_MouseClick);
            this.txtPatientNo.TextChanged += new System.EventHandler(this.txtPatientNo_TextChanged);
            this.txtPatientNo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPatientNo_KeyUp);
            this.txtPatientNo.MouseEnter += new System.EventHandler(this.txtPatientNo_MouseEnter);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(529, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(192, 25);
            this.label3.TabIndex = 19;
            this.label3.Text = "Patient Channelling Bill";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClose,
            this.btnNewn,
            this.btnList,
            this.btnSave,
            this.btnConfirm,
            this.toolStripButton4,
            this.btnPrint,
            this.btnVoid});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(729, 27);
            this.toolStrip1.TabIndex = 62;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // btnClose
            // 
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 24);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNewn
            // 
            this.btnNewn.Image = ((System.Drawing.Image)(resources.GetObject("btnNewn.Image")));
            this.btnNewn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewn.Name = "btnNewn";
            this.btnNewn.Size = new System.Drawing.Size(55, 24);
            this.btnNewn.Text = "New";
            this.btnNewn.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnList
            // 
            this.btnList.Image = ((System.Drawing.Image)(resources.GetObject("btnList.Image")));
            this.btnList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnList.Name = "btnList";
            this.btnList.Size = new System.Drawing.Size(49, 24);
            this.btnList.Text = "List";
            this.btnList.Click += new System.EventHandler(this.btnList_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 24);
            this.btnSave.Text = "Save/Print";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Enabled = false;
            this.btnConfirm.Image = global::UserAutherization.Properties.Resources.CAIJ0ZXM;
            this.btnConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 24);
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Enabled = false;
            this.toolStripButton4.Image = global::UserAutherization.Properties.Resources.images;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(51, 24);
            this.toolStripButton4.Text = "Edit";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(56, 24);
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnReprintScan_Click);
            // 
            // btnVoid
            // 
            this.btnVoid.Enabled = false;
            this.btnVoid.Image = ((System.Drawing.Image)(resources.GetObject("btnVoid.Image")));
            this.btnVoid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnVoid.Name = "btnVoid";
            this.btnVoid.Size = new System.Drawing.Size(54, 24);
            this.btnVoid.Text = "Void";
            this.btnVoid.Click += new System.EventHandler(this.btnVoid_Click);
            // 
            // frmChannellig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(729, 499);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(10, 80);
            this.Name = "frmChannellig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CHANNELLING";
            this.Activated += new System.EventHandler(this.frmScan_Activated);
            this.Load += new System.EventHandler(this.frmScan_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltrInsuranceCampany)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScanItems)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtContactNo;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.Label lblRemarks;
        private System.Windows.Forms.Label lblContactNo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtConsultant;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label lblCreditCardNo;
        private System.Windows.Forms.TextBox txtCreditCardNo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtTokenNo;
        private System.Windows.Forms.Button btnConsultant;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvScanItems;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtReceiptNo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.DateTimePicker dtpRepDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtnetTotal;
        private System.Windows.Forms.TextBox txtDisAmount;
        private System.Windows.Forms.TextBox txtdisRate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtPatientNo;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnClose;
        public System.Windows.Forms.ToolStripButton btnNewn;
        private System.Windows.Forms.ToolStripButton btnList;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripButton btnConfirm;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        internal DataSet DsScan;
        private System.Windows.Forms.CheckBox rbcash;
        private System.Windows.Forms.TextBox txtPationType;
        private System.Windows.Forms.ToolStripButton btnVoid;
        private System.Windows.Forms.TextBox txtTestType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Scanid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScanName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GlAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Duration;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConsultantFee;
        private System.Windows.Forms.DataGridViewTextBoxColumn HospitalFee;
        private System.Windows.Forms.DataGridViewTextBoxColumn Total;
        private System.Windows.Forms.DateTimePicker dtpTime;
        private System.Windows.Forms.Label lblSession1;
        private System.Windows.Forms.TextBox txtconsultIntime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSession;
        private System.Windows.Forms.Label label20;
        private Infragistics.Win.UltraWinGrid.UltraCombo UltrInsuranceCampany;
        private System.Windows.Forms.CheckBox cbNormalBill;
    }
}
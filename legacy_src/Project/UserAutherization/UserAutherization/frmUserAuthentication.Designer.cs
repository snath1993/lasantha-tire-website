namespace UserAutherization
{
    partial class frmUserAuthentication
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
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUserAuthentication));
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem10 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            this.cmbUser = new System.Windows.Forms.ComboBox();
            this.dgvAuthentication = new System.Windows.Forms.DataGridView();
            this.Activity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SystemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SelectActivity = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Run = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Add = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SpecialEdit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkforceinv = new System.Windows.Forms.CheckBox();
            this.combMode = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.chklockedWH = new System.Windows.Forms.CheckBox();
            this.chklockedRep = new System.Windows.Forms.CheckBox();
            this.chklockedPay = new System.Windows.Forms.CheckBox();
            this.chklockedtax = new System.Windows.Forms.CheckBox();
            this.cmbwh = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label24 = new System.Windows.Forms.Label();
            this.cmbRep = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label23 = new System.Windows.Forms.Label();
            this.cmbPayType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label22 = new System.Windows.Forms.Label();
            this.cmbInvoiceType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label21 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkforcediscount = new System.Windows.Forms.CheckBox();
            this.chkforceitqty = new System.Windows.Forms.CheckBox();
            this.chkforceunitprice = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuthentication)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.combMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbwh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPayType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbInvoiceType)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbUser
            // 
            this.cmbUser.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbUser.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbUser.FormattingEnabled = true;
            this.cmbUser.Location = new System.Drawing.Point(469, 21);
            this.cmbUser.Name = "cmbUser";
            this.cmbUser.Size = new System.Drawing.Size(187, 21);
            this.cmbUser.TabIndex = 0;
            this.cmbUser.Text = "SELECT USER";
            this.cmbUser.SelectedIndexChanged += new System.EventHandler(this.cmbUser_SelectedIndexChanged);
            // 
            // dgvAuthentication
            // 
            this.dgvAuthentication.BackgroundColor = System.Drawing.Color.White;
            this.dgvAuthentication.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuthentication.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Activity,
            this.SystemName,
            this.SelectActivity,
            this.Run,
            this.Add,
            this.Edit,
            this.Delete,
            this.SpecialEdit});
            this.dgvAuthentication.GridColor = System.Drawing.Color.DarkBlue;
            this.dgvAuthentication.Location = new System.Drawing.Point(9, 70);
            this.dgvAuthentication.Name = "dgvAuthentication";
            this.dgvAuthentication.RowHeadersVisible = false;
            this.dgvAuthentication.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvAuthentication.Size = new System.Drawing.Size(757, 289);
            this.dgvAuthentication.TabIndex = 1;
            // 
            // Activity
            // 
            this.Activity.HeaderText = "Activity Name";
            this.Activity.Name = "Activity";
            this.Activity.ReadOnly = true;
            this.Activity.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Activity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Activity.Width = 200;
            // 
            // SystemName
            // 
            this.SystemName.HeaderText = "Form Name";
            this.SystemName.Name = "SystemName";
            this.SystemName.Width = 175;
            // 
            // SelectActivity
            // 
            this.SelectActivity.HeaderText = "SelectActivity";
            this.SelectActivity.Name = "SelectActivity";
            this.SelectActivity.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SelectActivity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SelectActivity.Width = 80;
            // 
            // Run
            // 
            this.Run.HeaderText = "Run";
            this.Run.Name = "Run";
            this.Run.Width = 50;
            // 
            // Add
            // 
            this.Add.HeaderText = "Add";
            this.Add.Name = "Add";
            this.Add.Width = 50;
            // 
            // Edit
            // 
            this.Edit.HeaderText = "Edit";
            this.Edit.Name = "Edit";
            this.Edit.Width = 50;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.Width = 50;
            // 
            // SpecialEdit
            // 
            this.SpecialEdit.HeaderText = "Special Edit";
            this.SpecialEdit.Name = "SpecialEdit";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(565, 435);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(91, 27);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(565, 467);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopy.Location = new System.Drawing.Point(565, 403);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(91, 27);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkforceinv);
            this.groupBox1.Controls.Add(this.combMode);
            this.groupBox1.Controls.Add(this.ultraLabel1);
            this.groupBox1.Controls.Add(this.chklockedWH);
            this.groupBox1.Controls.Add(this.chklockedRep);
            this.groupBox1.Controls.Add(this.chklockedPay);
            this.groupBox1.Controls.Add(this.chklockedtax);
            this.groupBox1.Controls.Add(this.cmbwh);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.cmbRep);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.cmbPayType);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.cmbInvoiceType);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Location = new System.Drawing.Point(9, 363);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 163);
            this.groupBox1.TabIndex = 176;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Types Option";
            // 
            // chkforceinv
            // 
            this.chkforceinv.AutoSize = true;
            this.chkforceinv.Location = new System.Drawing.Point(282, 137);
            this.chkforceinv.Name = "chkforceinv";
            this.chkforceinv.Size = new System.Drawing.Size(53, 17);
            this.chkforceinv.TabIndex = 199;
            this.chkforceinv.Text = "Force";
            this.chkforceinv.UseVisualStyleBackColor = true;
            // 
            // combMode
            // 
            this.combMode.AutoSize = false;
            appearance13.Image = ((object)(resources.GetObject("appearance13.Image")));
            valueListItem2.Appearance = appearance13;
            valueListItem2.DataValue = ((long)(1));
            valueListItem2.DisplayText = "Cash";
            appearance12.Image = ((object)(resources.GetObject("appearance12.Image")));
            valueListItem1.Appearance = appearance12;
            valueListItem1.DataValue = ((long)(2));
            valueListItem1.DisplayText = "Credit";
            this.combMode.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem2,
            valueListItem1});
            this.combMode.Location = new System.Drawing.Point(142, 135);
            this.combMode.Name = "combMode";
            this.combMode.Size = new System.Drawing.Size(134, 21);
            this.combMode.TabIndex = 198;
            // 
            // ultraLabel1
            // 
            appearance15.BackColor = System.Drawing.Color.Transparent;
            appearance15.ForeColor = System.Drawing.Color.Red;
            appearance15.TextVAlignAsString = "Middle";
            this.ultraLabel1.Appearance = appearance15;
            this.ultraLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.Location = new System.Drawing.Point(14, 136);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(102, 21);
            this.ultraLabel1.TabIndex = 197;
            this.ultraLabel1.Text = "INVOICE MODE";
            // 
            // chklockedWH
            // 
            this.chklockedWH.AutoSize = true;
            this.chklockedWH.Location = new System.Drawing.Point(282, 108);
            this.chklockedWH.Name = "chklockedWH";
            this.chklockedWH.Size = new System.Drawing.Size(53, 17);
            this.chklockedWH.TabIndex = 196;
            this.chklockedWH.Text = "Force";
            this.chklockedWH.UseVisualStyleBackColor = true;
            // 
            // chklockedRep
            // 
            this.chklockedRep.AutoSize = true;
            this.chklockedRep.Location = new System.Drawing.Point(282, 79);
            this.chklockedRep.Name = "chklockedRep";
            this.chklockedRep.Size = new System.Drawing.Size(53, 17);
            this.chklockedRep.TabIndex = 195;
            this.chklockedRep.Text = "Force";
            this.chklockedRep.UseVisualStyleBackColor = true;
            // 
            // chklockedPay
            // 
            this.chklockedPay.AutoSize = true;
            this.chklockedPay.Location = new System.Drawing.Point(282, 49);
            this.chklockedPay.Name = "chklockedPay";
            this.chklockedPay.Size = new System.Drawing.Size(53, 17);
            this.chklockedPay.TabIndex = 194;
            this.chklockedPay.Text = "Force";
            this.chklockedPay.UseVisualStyleBackColor = true;
            // 
            // chklockedtax
            // 
            this.chklockedtax.AutoSize = true;
            this.chklockedtax.Location = new System.Drawing.Point(282, 19);
            this.chklockedtax.Name = "chklockedtax";
            this.chklockedtax.Size = new System.Drawing.Size(53, 17);
            this.chklockedtax.TabIndex = 193;
            this.chklockedtax.Text = "Force";
            this.chklockedtax.UseVisualStyleBackColor = true;
            // 
            // cmbwh
            // 
            this.cmbwh.AutoSize = false;
            this.cmbwh.Location = new System.Drawing.Point(142, 104);
            this.cmbwh.Name = "cmbwh";
            this.cmbwh.Size = new System.Drawing.Size(134, 21);
            this.cmbwh.TabIndex = 192;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(11, 108);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(110, 13);
            this.label24.TabIndex = 191;
            this.label24.Text = "Defalut Where House";
            // 
            // cmbRep
            // 
            this.cmbRep.AutoSize = false;
            this.cmbRep.Location = new System.Drawing.Point(142, 76);
            this.cmbRep.Name = "cmbRep";
            this.cmbRep.Size = new System.Drawing.Size(134, 21);
            this.cmbRep.TabIndex = 190;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(11, 77);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(64, 13);
            this.label23.TabIndex = 189;
            this.label23.Text = "Defalut Rep";
            // 
            // cmbPayType
            // 
            this.cmbPayType.AutoSize = false;
            appearance7.Image = ((object)(resources.GetObject("appearance7.Image")));
            valueListItem6.Appearance = appearance7;
            valueListItem6.DataValue = ((long)(2));
            valueListItem6.DisplayText = "Cash";
            appearance8.Image = ((object)(resources.GetObject("appearance8.Image")));
            valueListItem10.Appearance = appearance8;
            valueListItem10.DataValue = ((long)(1));
            valueListItem10.DisplayText = "Credit";
            appearance9.Image = ((object)(resources.GetObject("appearance9.Image")));
            valueListItem3.Appearance = appearance9;
            valueListItem3.DataValue = ((long)(3));
            valueListItem3.DisplayText = "Other";
            valueListItem5.DataValue = ((long)(4));
            valueListItem5.DisplayText = "Optional";
            this.cmbPayType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem6,
            valueListItem10,
            valueListItem3,
            valueListItem5});
            this.cmbPayType.Location = new System.Drawing.Point(142, 46);
            this.cmbPayType.Name = "cmbPayType";
            this.cmbPayType.Size = new System.Drawing.Size(134, 21);
            this.cmbPayType.TabIndex = 188;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(11, 50);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(89, 13);
            this.label22.TabIndex = 187;
            this.label22.Text = "Defalut Pay Type";
            // 
            // cmbInvoiceType
            // 
            this.cmbInvoiceType.AutoSize = false;
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            valueListItem7.Appearance = appearance4;
            valueListItem7.DataValue = ((long)(2));
            valueListItem7.DisplayText = "Exclusive";
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            valueListItem8.Appearance = appearance5;
            valueListItem8.DataValue = ((long)(1));
            valueListItem8.DisplayText = "Inclusive";
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            valueListItem9.Appearance = appearance6;
            valueListItem9.DataValue = ((long)(3));
            valueListItem9.DisplayText = "Non VAT";
            valueListItem4.DataValue = ((long)(4));
            valueListItem4.DisplayText = "Optional";
            this.cmbInvoiceType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem7,
            valueListItem8,
            valueListItem9,
            valueListItem4});
            this.cmbInvoiceType.Location = new System.Drawing.Point(142, 19);
            this.cmbInvoiceType.Name = "cmbInvoiceType";
            this.cmbInvoiceType.Size = new System.Drawing.Size(134, 21);
            this.cmbInvoiceType.TabIndex = 186;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(11, 22);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(89, 13);
            this.label21.TabIndex = 185;
            this.label21.Text = "Defalut Tax Type";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkforcediscount);
            this.groupBox2.Controls.Add(this.chkforceitqty);
            this.groupBox2.Controls.Add(this.chkforceunitprice);
            this.groupBox2.Location = new System.Drawing.Point(438, 365);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(121, 134);
            this.groupBox2.TabIndex = 177;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Invoice Settings";
            // 
            // chkforcediscount
            // 
            this.chkforcediscount.AutoSize = true;
            this.chkforcediscount.Location = new System.Drawing.Point(40, 64);
            this.chkforcediscount.Name = "chkforcediscount";
            this.chkforcediscount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkforcediscount.Size = new System.Drawing.Size(68, 17);
            this.chkforcediscount.TabIndex = 201;
            this.chkforcediscount.Text = "Discount";
            this.chkforcediscount.UseVisualStyleBackColor = true;
            // 
            // chkforceitqty
            // 
            this.chkforceitqty.AutoSize = true;
            this.chkforceitqty.Location = new System.Drawing.Point(13, 41);
            this.chkforceitqty.Name = "chkforceitqty";
            this.chkforceitqty.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkforceitqty.Size = new System.Drawing.Size(95, 17);
            this.chkforceitqty.TabIndex = 200;
            this.chkforceitqty.Text = "Force Item Qty";
            this.chkforceitqty.UseVisualStyleBackColor = true;
            // 
            // chkforceunitprice
            // 
            this.chkforceunitprice.AutoSize = true;
            this.chkforceunitprice.Location = new System.Drawing.Point(6, 18);
            this.chkforceunitprice.Name = "chkforceunitprice";
            this.chkforceunitprice.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkforceunitprice.Size = new System.Drawing.Size(102, 17);
            this.chkforceunitprice.TabIndex = 199;
            this.chkforceunitprice.Text = "Force Unit Price";
            this.chkforceunitprice.UseVisualStyleBackColor = true;
            // 
            // frmUserAuthentication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(775, 531);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvAuthentication);
            this.Controls.Add(this.cmbUser);
            this.MaximizeBox = false;
            this.Name = "frmUserAuthentication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Authentications";
            this.Load += new System.EventHandler(this.frmUserAuthentication_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuthentication)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.combMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbwh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPayType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbInvoiceType)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbUser;
        private System.Windows.Forms.DataGridView dgvAuthentication;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.DataGridViewTextBoxColumn Activity;
        private System.Windows.Forms.DataGridViewTextBoxColumn SystemName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectActivity;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Run;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Add;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Edit;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Delete;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chklockedWH;
        private System.Windows.Forms.CheckBox chklockedRep;
        private System.Windows.Forms.CheckBox chklockedPay;
        private System.Windows.Forms.CheckBox chklockedtax;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cmbwh;
        private System.Windows.Forms.Label label24;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cmbRep;
        private System.Windows.Forms.Label label23;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cmbPayType;
        private System.Windows.Forms.Label label22;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cmbInvoiceType;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkforceitqty;
        private System.Windows.Forms.CheckBox chkforceunitprice;
        private System.Windows.Forms.CheckBox chkforcediscount;
        private System.Windows.Forms.CheckBox chkforceinv;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor combMode;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SpecialEdit;
    }
}
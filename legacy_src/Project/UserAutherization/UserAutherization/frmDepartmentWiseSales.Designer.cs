namespace UserAutherization
{
    partial class frmDepartmentWiseSales
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDepartmentWiseSales));
            this.optdep = new System.Windows.Forms.RadioButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.dtfrom = new System.Windows.Forms.DateTimePicker();
            this.dtto = new System.Windows.Forms.DateTimePicker();
            this.optdate = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rdInv = new System.Windows.Forms.RadioButton();
            this.rdodipsum = new System.Windows.Forms.RadioButton();
            this.cmbAR = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.label3 = new System.Windows.Forms.Label();
            this.optind = new System.Windows.Forms.RadioButton();
            this.rdodetail = new System.Windows.Forms.RadioButton();
            this.rdoVat = new System.Windows.Forms.RadioButton();
            this.cmbtype = new System.Windows.Forms.ComboBox();
            this.rdovatclamed = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbvatcon = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.cmbnbtcon = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdosale = new System.Windows.Forms.RadioButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbvatcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbnbtcon)).BeginInit();
            this.SuspendLayout();
            // 
            // optdep
            // 
            this.optdep.AutoSize = true;
            this.optdep.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optdep.Location = new System.Drawing.Point(12, 95);
            this.optdep.Name = "optdep";
            this.optdep.Size = new System.Drawing.Size(156, 17);
            this.optdep.TabIndex = 0;
            this.optdep.TabStop = true;
            this.optdep.Text = "DEPARTMENT WISE SALES ";
            this.optdep.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClear,
            this.btnRefresh,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(626, 25);
            this.toolStrip1.TabIndex = 136;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnClear
            // 
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(52, 22);
            this.btnClear.Text = "Print";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(66, 22);
            this.btnRefresh.Text = "Refresh";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::UserAutherization.Properties.Resources.refer_site;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(147, 22);
            this.toolStripButton1.Text = "Import Data from Sage";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // dtfrom
            // 
            this.dtfrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtfrom.Location = new System.Drawing.Point(48, 39);
            this.dtfrom.Name = "dtfrom";
            this.dtfrom.Size = new System.Drawing.Size(108, 20);
            this.dtfrom.TabIndex = 302;
            this.dtfrom.Value = new System.DateTime(2013, 11, 7, 0, 0, 0, 0);
            // 
            // dtto
            // 
            this.dtto.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtto.Location = new System.Drawing.Point(198, 39);
            this.dtto.Name = "dtto";
            this.dtto.Size = new System.Drawing.Size(108, 20);
            this.dtto.TabIndex = 303;
            this.dtto.Value = new System.DateTime(2013, 11, 7, 0, 0, 0, 0);
            // 
            // optdate
            // 
            this.optdate.AutoSize = true;
            this.optdate.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optdate.Location = new System.Drawing.Point(12, 118);
            this.optdate.Name = "optdate";
            this.optdate.Size = new System.Drawing.Size(113, 17);
            this.optdate.TabIndex = 304;
            this.optdate.TabStop = true;
            this.optdate.Text = "DATE WISE SALES";
            this.optdate.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 305;
            this.label1.Text = "FROM";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(162, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 306;
            this.label2.Text = "TO";
            // 
            // rdInv
            // 
            this.rdInv.AutoSize = true;
            this.rdInv.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdInv.Location = new System.Drawing.Point(12, 141);
            this.rdInv.Name = "rdInv";
            this.rdInv.Size = new System.Drawing.Size(129, 17);
            this.rdInv.TabIndex = 307;
            this.rdInv.TabStop = true;
            this.rdInv.Text = "INVOICE WISE SALES";
            this.rdInv.UseVisualStyleBackColor = true;
            // 
            // rdodipsum
            // 
            this.rdodipsum.AutoSize = true;
            this.rdodipsum.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdodipsum.Location = new System.Drawing.Point(12, 164);
            this.rdodipsum.Name = "rdodipsum";
            this.rdodipsum.Size = new System.Drawing.Size(255, 17);
            this.rdodipsum.TabIndex = 308;
            this.rdodipsum.TabStop = true;
            this.rdodipsum.Text = "MONTHLY DEPARTMENT WISE SALES SUMMARY";
            this.rdodipsum.UseVisualStyleBackColor = true;
            // 
            // cmbAR
            // 
            this.cmbAR.CheckedListSettings.CheckStateMember = "";
            this.cmbAR.Location = new System.Drawing.Point(126, 65);
            this.cmbAR.Name = "cmbAR";
            this.cmbAR.Size = new System.Drawing.Size(180, 22);
            this.cmbAR.TabIndex = 309;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 310;
            this.label3.Text = "DEPARTMENT";
            // 
            // optind
            // 
            this.optind.AutoSize = true;
            this.optind.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optind.Location = new System.Drawing.Point(12, 187);
            this.optind.Name = "optind";
            this.optind.Size = new System.Drawing.Size(185, 17);
            this.optind.TabIndex = 311;
            this.optind.TabStop = true;
            this.optind.Text = "INDIVIDUAL DEPARTMENT SALES";
            this.optind.UseVisualStyleBackColor = true;
            // 
            // rdodetail
            // 
            this.rdodetail.AutoSize = true;
            this.rdodetail.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdodetail.Location = new System.Drawing.Point(12, 210);
            this.rdodetail.Name = "rdodetail";
            this.rdodetail.Size = new System.Drawing.Size(190, 17);
            this.rdodetail.TabIndex = 312;
            this.rdodetail.TabStop = true;
            this.rdodetail.Text = "INDIVIDUAL DEPARTMENT DETAIL";
            this.rdodetail.UseVisualStyleBackColor = true;
            // 
            // rdoVat
            // 
            this.rdoVat.AutoSize = true;
            this.rdoVat.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoVat.Location = new System.Drawing.Point(327, 95);
            this.rdoVat.Name = "rdoVat";
            this.rdoVat.Size = new System.Drawing.Size(87, 17);
            this.rdoVat.TabIndex = 313;
            this.rdoVat.TabStop = true;
            this.rdoVat.Text = "VAT REPORT";
            this.rdoVat.UseVisualStyleBackColor = true;
            // 
            // cmbtype
            // 
            this.cmbtype.FormattingEnabled = true;
            this.cmbtype.Items.AddRange(new object[] {
            "Cash",
            "Credit"});
            this.cmbtype.Location = new System.Drawing.Point(496, 95);
            this.cmbtype.Name = "cmbtype";
            this.cmbtype.Size = new System.Drawing.Size(121, 21);
            this.cmbtype.TabIndex = 314;
            // 
            // rdovatclamed
            // 
            this.rdovatclamed.AutoSize = true;
            this.rdovatclamed.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdovatclamed.Location = new System.Drawing.Point(327, 118);
            this.rdovatclamed.Name = "rdovatclamed";
            this.rdovatclamed.Size = new System.Drawing.Size(290, 17);
            this.rdovatclamed.TabIndex = 315;
            this.rdovatclamed.TabStop = true;
            this.rdovatclamed.Text = "V.A.T. CLAIMED REPORT (RAW MATERIAL PURCHASE)";
            this.rdovatclamed.UseVisualStyleBackColor = true;
            this.rdovatclamed.CheckedChanged += new System.EventHandler(this.rdovatclamed_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.Location = new System.Drawing.Point(327, 141);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(150, 17);
            this.radioButton1.TabIndex = 316;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "OTHER LOCAL PURCHASE";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(454, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 317;
            this.label4.Text = "TYPE";
            // 
            // cmbvatcon
            // 
            this.cmbvatcon.CheckedListSettings.CheckStateMember = "";
            this.cmbvatcon.Location = new System.Drawing.Point(437, 37);
            this.cmbvatcon.Name = "cmbvatcon";
            this.cmbvatcon.Size = new System.Drawing.Size(180, 22);
            this.cmbvatcon.TabIndex = 318;
            // 
            // cmbnbtcon
            // 
            this.cmbnbtcon.CheckedListSettings.CheckStateMember = "";
            this.cmbnbtcon.Location = new System.Drawing.Point(437, 62);
            this.cmbnbtcon.Name = "cmbnbtcon";
            this.cmbnbtcon.Size = new System.Drawing.Size(180, 22);
            this.cmbnbtcon.TabIndex = 319;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(328, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 320;
            this.label5.Text = "VAT CONTROL";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(328, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 321;
            this.label6.Text = "NBT CONTROL";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.groupBox1.Location = new System.Drawing.Point(312, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(10, 225);
            this.groupBox1.TabIndex = 322;
            this.groupBox1.TabStop = false;
            // 
            // rdosale
            // 
            this.rdosale.AutoSize = true;
            this.rdosale.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdosale.Location = new System.Drawing.Point(174, 95);
            this.rdosale.Name = "rdosale";
            this.rdosale.Size = new System.Drawing.Size(72, 17);
            this.rdosale.TabIndex = 323;
            this.rdosale.TabStop = true;
            this.rdosale.Text = "ITEM QTY";
            this.rdosale.UseVisualStyleBackColor = true;
            // 
            // frmDepartmentWiseSales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 238);
            this.Controls.Add(this.rdosale);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbnbtcon);
            this.Controls.Add(this.cmbvatcon);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.rdovatclamed);
            this.Controls.Add(this.cmbtype);
            this.Controls.Add(this.rdoVat);
            this.Controls.Add(this.rdodetail);
            this.Controls.Add(this.optind);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbAR);
            this.Controls.Add(this.rdodipsum);
            this.Controls.Add(this.rdInv);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.optdate);
            this.Controls.Add(this.dtto);
            this.Controls.Add(this.dtfrom);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.optdep);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmDepartmentWiseSales";
            this.Text = "Financial Reports";
            this.Load += new System.EventHandler(this.frmDepartmentWiseSales_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbvatcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbnbtcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton optdep;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.DateTimePicker dtfrom;
        private System.Windows.Forms.DateTimePicker dtto;
        private System.Windows.Forms.RadioButton optdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.RadioButton rdInv;
        private System.Windows.Forms.RadioButton rdodipsum;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbAR;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton optind;
        private System.Windows.Forms.RadioButton rdodetail;
        private System.Windows.Forms.RadioButton rdoVat;
        private System.Windows.Forms.ComboBox cmbtype;
        private System.Windows.Forms.RadioButton rdovatclamed;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label4;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbvatcon;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbnbtcon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdosale;
    }
}
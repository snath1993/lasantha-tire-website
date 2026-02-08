namespace UserAutherization
{
    partial class frmSettingsTax
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettingsTax));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.cmbTaxID = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.txttaxName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtrate = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.txtRank = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.rbttaxOnTax = new System.Windows.Forms.RadioButton();
            this.rbtIndTax = new System.Windows.Forms.RadioButton();
            this.chkIsActive = new System.Windows.Forms.CheckBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTaxID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txttaxName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtrate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRank)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(298, 25);
            this.toolStrip1.TabIndex = 136;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 22);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbTaxID
            // 
            this.cmbTaxID.CheckedListSettings.CheckStateMember = "";
            this.cmbTaxID.Location = new System.Drawing.Point(95, 52);
            this.cmbTaxID.Name = "cmbTaxID";
            this.cmbTaxID.Size = new System.Drawing.Size(100, 23);
            this.cmbTaxID.TabIndex = 137;
            this.cmbTaxID.RowSelected += new Infragistics.Win.UltraWinGrid.RowSelectedEventHandler(this.cmbTaxID_RowSelected);
            // 
            // txttaxName
            // 
            this.txttaxName.Location = new System.Drawing.Point(95, 78);
            this.txttaxName.Name = "txttaxName";
            this.txttaxName.Size = new System.Drawing.Size(100, 22);
            this.txttaxName.TabIndex = 138;
            // 
            // txtrate
            // 
            this.txtrate.Location = new System.Drawing.Point(95, 103);
            this.txtrate.Name = "txtrate";
            this.txtrate.Size = new System.Drawing.Size(100, 22);
            this.txtrate.TabIndex = 139;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 140;
            this.label1.Text = "Tax ID";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label61.Location = new System.Drawing.Point(33, 194);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(33, 15);
            this.label61.TabIndex = 146;
            this.label61.Text = "Rank";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label58.Location = new System.Drawing.Point(33, 107);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(30, 15);
            this.label58.TabIndex = 147;
            this.label58.Text = "Rate";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label59.Location = new System.Drawing.Point(33, 81);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(57, 15);
            this.label59.TabIndex = 145;
            this.label59.Text = "TaxName";
            // 
            // txtRank
            // 
            this.txtRank.Location = new System.Drawing.Point(95, 194);
            this.txtRank.Name = "txtRank";
            this.txtRank.Size = new System.Drawing.Size(100, 22);
            this.txtRank.TabIndex = 149;
            // 
            // rbttaxOnTax
            // 
            this.rbttaxOnTax.AutoSize = true;
            this.rbttaxOnTax.Location = new System.Drawing.Point(33, 132);
            this.rbttaxOnTax.Name = "rbttaxOnTax";
            this.rbttaxOnTax.Size = new System.Drawing.Size(162, 17);
            this.rbttaxOnTax.TabIndex = 150;
            this.rbttaxOnTax.TabStop = true;
            this.rbttaxOnTax.Text = "Is Tax onTax Applicable";
            this.rbttaxOnTax.UseVisualStyleBackColor = true;
            this.rbttaxOnTax.CheckedChanged += new System.EventHandler(this.rbttaxOnTax_CheckedChanged);
            // 
            // rbtIndTax
            // 
            this.rbtIndTax.AutoSize = true;
            this.rbtIndTax.Location = new System.Drawing.Point(33, 158);
            this.rbtIndTax.Name = "rbtIndTax";
            this.rbtIndTax.Size = new System.Drawing.Size(137, 17);
            this.rbtIndTax.TabIndex = 152;
            this.rbtIndTax.TabStop = true;
            this.rbtIndTax.Text = "Is Independent Tax";
            this.rbtIndTax.UseVisualStyleBackColor = true;
            this.rbtIndTax.CheckedChanged += new System.EventHandler(this.rbtIndTax_CheckedChanged);
            // 
            // chkIsActive
            // 
            this.chkIsActive.AutoSize = true;
            this.chkIsActive.Location = new System.Drawing.Point(201, 55);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Size = new System.Drawing.Size(76, 17);
            this.chkIsActive.TabIndex = 153;
            this.chkIsActive.Text = "Is Active";
            this.chkIsActive.UseVisualStyleBackColor = true;
            // 
            // frmSettingsTax
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 294);
            this.Controls.Add(this.chkIsActive);
            this.Controls.Add(this.rbtIndTax);
            this.Controls.Add(this.rbttaxOnTax);
            this.Controls.Add(this.txtRank);
            this.Controls.Add(this.label61);
            this.Controls.Add(this.label58);
            this.Controls.Add(this.label59);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtrate);
            this.Controls.Add(this.txttaxName);
            this.Controls.Add(this.cmbTaxID);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSettingsTax";
            this.Text = "Tax Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTaxID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txttaxName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtrate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRank)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbTaxID;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txttaxName;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtrate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label59;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtRank;
        private System.Windows.Forms.RadioButton rbttaxOnTax;
        private System.Windows.Forms.RadioButton rbtIndTax;
        private System.Windows.Forms.CheckBox chkIsActive;

    }
}
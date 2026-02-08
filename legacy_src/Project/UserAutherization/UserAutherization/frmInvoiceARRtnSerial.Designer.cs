namespace UserAutherization
{
    partial class frmInvoiceARRtnSerial
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
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtQty = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cbSerials = new System.Windows.Forms.CheckedListBox();
            this.txtWH = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtItemDescription = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtItemID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemID)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.txtQty);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox1.Controls.Add(this.cbSerials);
            this.ultraGroupBox1.Controls.Add(this.txtWH);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox1.Controls.Add(this.txtItemDescription);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.txtItemID);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel7);
            this.ultraGroupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGroupBox1.Location = new System.Drawing.Point(1, 3);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(289, 268);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Serial No\'s";
            // 
            // txtQty
            // 
            this.txtQty.AutoSize = false;
            this.txtQty.FormatString = "0";
            this.txtQty.Location = new System.Drawing.Point(101, 86);
            this.txtQty.Name = "txtQty";
            this.txtQty.NullText = "0";
            this.txtQty.PromptChar = ' ';
            this.txtQty.ReadOnly = true;
            this.txtQty.Size = new System.Drawing.Size(100, 20);
            this.txtQty.TabIndex = 19;
            // 
            // ultraLabel3
            // 
            appearance1.TextVAlignAsString = "Middle";
            this.ultraLabel3.Appearance = appearance1;
            this.ultraLabel3.Location = new System.Drawing.Point(7, 83);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(89, 21);
            this.ultraLabel3.TabIndex = 18;
            this.ultraLabel3.Text = "Quantity";
            // 
            // cbSerials
            // 
            this.cbSerials.CheckOnClick = true;
            this.cbSerials.FormattingEnabled = true;
            this.cbSerials.Location = new System.Drawing.Point(6, 107);
            this.cbSerials.Name = "cbSerials";
            this.cbSerials.Size = new System.Drawing.Size(273, 148);
            this.cbSerials.TabIndex = 17;
            // 
            // txtWH
            // 
            this.txtWH.AutoSize = false;
            this.txtWH.Location = new System.Drawing.Point(101, 64);
            this.txtWH.Name = "txtWH";
            this.txtWH.ReadOnly = true;
            this.txtWH.Size = new System.Drawing.Size(178, 21);
            this.txtWH.TabIndex = 16;
            // 
            // ultraLabel2
            // 
            appearance2.TextVAlignAsString = "Middle";
            this.ultraLabel2.Appearance = appearance2;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 64);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(89, 21);
            this.ultraLabel2.TabIndex = 15;
            this.ultraLabel2.Text = "WH";
            // 
            // txtItemDescription
            // 
            this.txtItemDescription.AutoSize = false;
            this.txtItemDescription.Location = new System.Drawing.Point(101, 42);
            this.txtItemDescription.Name = "txtItemDescription";
            this.txtItemDescription.ReadOnly = true;
            this.txtItemDescription.Size = new System.Drawing.Size(178, 21);
            this.txtItemDescription.TabIndex = 14;
            // 
            // ultraLabel1
            // 
            appearance3.TextVAlignAsString = "Middle";
            this.ultraLabel1.Appearance = appearance3;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 42);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(89, 21);
            this.ultraLabel1.TabIndex = 13;
            this.ultraLabel1.Text = "Item Description";
            // 
            // txtItemID
            // 
            this.txtItemID.AutoSize = false;
            this.txtItemID.Location = new System.Drawing.Point(101, 20);
            this.txtItemID.Name = "txtItemID";
            this.txtItemID.ReadOnly = true;
            this.txtItemID.Size = new System.Drawing.Size(178, 21);
            this.txtItemID.TabIndex = 12;
            // 
            // ultraLabel7
            // 
            appearance4.TextVAlignAsString = "Middle";
            this.ultraLabel7.Appearance = appearance4;
            this.ultraLabel7.Location = new System.Drawing.Point(6, 20);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(89, 21);
            this.ultraLabel7.TabIndex = 11;
            this.ultraLabel7.Text = "Item ID";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(124, 279);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(205, 279);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmInvoiceARRtnSerial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 312);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ultraGroupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInvoiceARRtnSerial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmInvoiceARRtnSerial";
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemID)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private System.Windows.Forms.CheckedListBox cbSerials;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        public Infragistics.Win.UltraWinEditors.UltraTextEditor txtWH;
        public Infragistics.Win.UltraWinEditors.UltraTextEditor txtItemDescription;
        public Infragistics.Win.UltraWinEditors.UltraTextEditor txtItemID;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        public Infragistics.Win.UltraWinEditors.UltraNumericEditor txtQty;
    }
}
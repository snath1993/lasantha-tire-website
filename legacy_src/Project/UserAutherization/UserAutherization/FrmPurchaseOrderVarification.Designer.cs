namespace UserAutherization
{
    partial class FrmPurchaseOrderVarification
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
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPurchaseOrderVarification));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbType = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbType)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Location = new System.Drawing.Point(410, 137);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(49, 32);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.Color.Transparent;
            this.btnView.Location = new System.Drawing.Point(352, 137);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(53, 32);
            this.btnView.TabIndex = 3;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(315, 19);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(124, 21);
            this.dtpToDate.TabIndex = 0;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(83, 19);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(121, 21);
            this.dtpFromDate.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "From Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "To Date";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpToDate);
            this.groupBox1.Controls.Add(this.dtpFromDate);
            this.groupBox1.Location = new System.Drawing.Point(6, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(453, 58);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Date Range";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbType);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.btnClose);
            this.groupBox2.Controls.Add(this.btnView);
            this.groupBox2.Location = new System.Drawing.Point(8, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(471, 184);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(132, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Type";
            // 
            // cmbType
            // 
            appearance28.BackColor = System.Drawing.Color.White;
            this.cmbType.Appearance = appearance28;
            this.cmbType.CheckedListSettings.CheckStateMember = "";
            appearance29.BackColor = System.Drawing.SystemColors.Window;
            appearance29.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbType.DisplayLayout.Appearance = appearance29;
            this.cmbType.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.cmbType.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance30.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance30.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance30.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance30.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbType.DisplayLayout.GroupByBox.Appearance = appearance30;
            appearance31.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbType.DisplayLayout.GroupByBox.BandLabelAppearance = appearance31;
            this.cmbType.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance32.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance32.BackColor2 = System.Drawing.SystemColors.Control;
            appearance32.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance32.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbType.DisplayLayout.GroupByBox.PromptAppearance = appearance32;
            this.cmbType.DisplayLayout.MaxColScrollRegions = 1;
            this.cmbType.DisplayLayout.MaxRowScrollRegions = 1;
            appearance33.BackColor = System.Drawing.SystemColors.Window;
            appearance33.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbType.DisplayLayout.Override.ActiveCellAppearance = appearance33;
            appearance34.BackColor = System.Drawing.SystemColors.Highlight;
            appearance34.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.cmbType.DisplayLayout.Override.ActiveRowAppearance = appearance34;
            this.cmbType.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.cmbType.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.cmbType.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance35.BackColor = System.Drawing.SystemColors.Window;
            this.cmbType.DisplayLayout.Override.CardAreaAppearance = appearance35;
            appearance36.BorderColor = System.Drawing.Color.Silver;
            appearance36.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.cmbType.DisplayLayout.Override.CellAppearance = appearance36;
            this.cmbType.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.cmbType.DisplayLayout.Override.CellPadding = 0;
            appearance37.BackColor = System.Drawing.SystemColors.Control;
            appearance37.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance37.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance37.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance37.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbType.DisplayLayout.Override.GroupByRowAppearance = appearance37;
            appearance38.TextHAlignAsString = "Left";
            this.cmbType.DisplayLayout.Override.HeaderAppearance = appearance38;
            this.cmbType.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.cmbType.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance39.BackColor = System.Drawing.SystemColors.Window;
            appearance39.BorderColor = System.Drawing.Color.Silver;
            this.cmbType.DisplayLayout.Override.RowAppearance = appearance39;
            this.cmbType.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance40.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cmbType.DisplayLayout.Override.TemplateAddRowAppearance = appearance40;
            this.cmbType.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.cmbType.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.cmbType.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.cmbType.Location = new System.Drawing.Point(172, 94);
            this.cmbType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(136, 23);
            this.cmbType.TabIndex = 59;
            this.cmbType.TabStop = false;
            // 
            // FrmPurchaseOrderVarification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(489, 219);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmPurchaseOrderVarification";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase Order Status";
            this.Load += new System.EventHandler(this.FrmPurchaseOrderVarification_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbType;
    }
}
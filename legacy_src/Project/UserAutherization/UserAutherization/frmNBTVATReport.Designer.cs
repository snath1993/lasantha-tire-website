namespace UserAutherization
{
    partial class frmNBTVATReport
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
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNBTVATReport));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chbServiceItems = new System.Windows.Forms.CheckBox();
            this.chbStockcItems = new System.Windows.Forms.CheckBox();
            this.chkItemAll = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbItem = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.btnViewItemWise = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItem)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.dtpFromDate);
            this.groupBox3.Controls.Add(this.dtpToDate);
            this.groupBox3.Location = new System.Drawing.Point(12, 40);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(471, 55);
            this.groupBox3.TabIndex = 130;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select Date Range";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "From Date";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(250, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "To Date";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(87, 19);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(148, 20);
            this.dtpFromDate.TabIndex = 3;
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(311, 18);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(152, 20);
            this.dtpToDate.TabIndex = 4;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chbServiceItems);
            this.groupBox5.Controls.Add(this.chbStockcItems);
            this.groupBox5.Controls.Add(this.chkItemAll);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.cmbItem);
            this.groupBox5.Enabled = false;
            this.groupBox5.Location = new System.Drawing.Point(12, 101);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(471, 113);
            this.groupBox5.TabIndex = 129;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Select Item";
            // 
            // chbServiceItems
            // 
            this.chbServiceItems.AutoSize = true;
            this.chbServiceItems.Checked = true;
            this.chbServiceItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbServiceItems.Location = new System.Drawing.Point(240, 19);
            this.chbServiceItems.Name = "chbServiceItems";
            this.chbServiceItems.Size = new System.Drawing.Size(90, 17);
            this.chbServiceItems.TabIndex = 68;
            this.chbServiceItems.Text = "Service Items";
            this.chbServiceItems.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.chbServiceItems.UseVisualStyleBackColor = true;
            this.chbServiceItems.CheckedChanged += new System.EventHandler(this.chbServiceItems_CheckedChanged);
            // 
            // chbStockcItems
            // 
            this.chbStockcItems.AutoSize = true;
            this.chbStockcItems.Checked = true;
            this.chbStockcItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbStockcItems.Location = new System.Drawing.Point(126, 19);
            this.chbStockcItems.Name = "chbStockcItems";
            this.chbStockcItems.Size = new System.Drawing.Size(82, 17);
            this.chbStockcItems.TabIndex = 67;
            this.chbStockcItems.Text = "Stock Items";
            this.chbStockcItems.UseVisualStyleBackColor = true;
            this.chbStockcItems.CheckedChanged += new System.EventHandler(this.chbStockcItems_CheckedChanged);
            // 
            // chkItemAll
            // 
            this.chkItemAll.AutoSize = true;
            this.chkItemAll.Location = new System.Drawing.Point(356, 56);
            this.chkItemAll.Name = "chkItemAll";
            this.chkItemAll.Size = new System.Drawing.Size(65, 17);
            this.chkItemAll.TabIndex = 65;
            this.chkItemAll.Text = "All Items";
            this.chkItemAll.UseVisualStyleBackColor = true;
            this.chkItemAll.CheckedChanged += new System.EventHandler(this.chkItemAll_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(35, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 65;
            this.label6.Text = "Item";
            // 
            // cmbItem
            // 
            this.cmbItem.CheckedListSettings.CheckStateMember = "";
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbItem.DisplayLayout.Appearance = appearance1;
            this.cmbItem.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.cmbItem.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbItem.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.cmbItem.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbItem.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.cmbItem.DisplayLayout.MaxColScrollRegions = 1;
            this.cmbItem.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbItem.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.cmbItem.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.cmbItem.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.cmbItem.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.cmbItem.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.cmbItem.DisplayLayout.Override.CellAppearance = appearance8;
            this.cmbItem.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.cmbItem.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.cmbItem.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.cmbItem.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.cmbItem.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.cmbItem.DisplayLayout.Override.RowAppearance = appearance11;
            this.cmbItem.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cmbItem.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.cmbItem.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.cmbItem.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.cmbItem.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.cmbItem.Location = new System.Drawing.Point(84, 53);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(266, 22);
            this.cmbItem.TabIndex = 66;
            // 
            // btnViewItemWise
            // 
            this.btnViewItemWise.Location = new System.Drawing.Point(323, 231);
            this.btnViewItemWise.Name = "btnViewItemWise";
            this.btnViewItemWise.Size = new System.Drawing.Size(75, 23);
            this.btnViewItemWise.TabIndex = 67;
            this.btnViewItemWise.Text = "View";
            this.btnViewItemWise.UseVisualStyleBackColor = true;
            this.btnViewItemWise.Click += new System.EventHandler(this.btnViewItemWise_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(408, 231);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 131;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(137, 12);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(68, 17);
            this.radioButton1.TabIndex = 132;
            this.radioButton1.TabStop = true;
            this.radioButton1.Tag = "frmNBTVATInvWiseReportView ";
            this.radioButton1.Text = "Summery";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(265, 12);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(52, 17);
            this.radioButton2.TabIndex = 133;
            this.radioButton2.Tag = "frmNBTVATInvWiseReportView ";
            this.radioButton2.Text = "Detail";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // frmNBTVATReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 263);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnViewItemWise);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmNBTVATReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmNBTVATReport";
            this.Load += new System.EventHandler(this.frmNBTVATReport_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItem)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkItemAll;
        private System.Windows.Forms.Label label6;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbItem;
        private System.Windows.Forms.Button btnViewItemWise;
        private System.Windows.Forms.CheckBox chbServiceItems;
        private System.Windows.Forms.CheckBox chbStockcItems;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}
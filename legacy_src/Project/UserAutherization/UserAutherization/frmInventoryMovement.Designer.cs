namespace UserAutherization
{
    partial class frmInventoryMovement
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
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
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
            this.chkDateRange = new System.Windows.Forms.CheckBox();
            this.cmbWarehouse = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.ChkWarehouse = new System.Windows.Forms.CheckBox();
            this.cmbDocType = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.cmbItem = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.ChkTranType = new System.Windows.Forms.CheckBox();
            this.CheckItemID = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.rbtAll = new System.Windows.Forms.RadioButton();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.cmbWarehouse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDocType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItem)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkDateRange
            // 
            this.chkDateRange.AutoSize = true;
            this.chkDateRange.Location = new System.Drawing.Point(16, 50);
            this.chkDateRange.Name = "chkDateRange";
            this.chkDateRange.Size = new System.Drawing.Size(93, 17);
            this.chkDateRange.TabIndex = 62;
            this.chkDateRange.Text = "Date Range";
            this.chkDateRange.UseVisualStyleBackColor = true;
            this.chkDateRange.CheckedChanged += new System.EventHandler(this.chkDateRange_CheckedChanged);
            // 
            // cmbWarehouse
            // 
            appearance1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbWarehouse.Appearance = appearance1;
            this.cmbWarehouse.CheckedListSettings.CheckStateMember = "";
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbWarehouse.DisplayLayout.Appearance = appearance2;
            this.cmbWarehouse.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.cmbWarehouse.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbWarehouse.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbWarehouse.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.cmbWarehouse.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbWarehouse.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.cmbWarehouse.DisplayLayout.MaxColScrollRegions = 1;
            this.cmbWarehouse.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbWarehouse.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.cmbWarehouse.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.cmbWarehouse.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.cmbWarehouse.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.cmbWarehouse.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.cmbWarehouse.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.cmbWarehouse.DisplayLayout.Override.CellAppearance = appearance9;
            this.cmbWarehouse.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.cmbWarehouse.DisplayLayout.Override.CellPadding = 0;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbWarehouse.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.cmbWarehouse.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.cmbWarehouse.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.cmbWarehouse.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.cmbWarehouse.DisplayLayout.Override.RowAppearance = appearance12;
            this.cmbWarehouse.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cmbWarehouse.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.cmbWarehouse.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.cmbWarehouse.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.cmbWarehouse.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.cmbWarehouse.Location = new System.Drawing.Point(122, 135);
            this.cmbWarehouse.Name = "cmbWarehouse";
            this.cmbWarehouse.Size = new System.Drawing.Size(371, 23);
            this.cmbWarehouse.TabIndex = 61;
            // 
            // ChkWarehouse
            // 
            this.ChkWarehouse.AutoSize = true;
            this.ChkWarehouse.Location = new System.Drawing.Point(16, 141);
            this.ChkWarehouse.Name = "ChkWarehouse";
            this.ChkWarehouse.Size = new System.Drawing.Size(90, 17);
            this.ChkWarehouse.TabIndex = 60;
            this.ChkWarehouse.Text = "Warehouse";
            this.ChkWarehouse.UseVisualStyleBackColor = true;
            this.ChkWarehouse.CheckedChanged += new System.EventHandler(this.ChkWarehouse_CheckedChanged);
            // 
            // cmbDocType
            // 
            appearance14.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbDocType.Appearance = appearance14;
            this.cmbDocType.CheckedListSettings.CheckStateMember = "";
            appearance15.BackColor = System.Drawing.SystemColors.Window;
            appearance15.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbDocType.DisplayLayout.Appearance = appearance15;
            this.cmbDocType.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.cmbDocType.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance16.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance16.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance16.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbDocType.DisplayLayout.GroupByBox.Appearance = appearance16;
            appearance17.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbDocType.DisplayLayout.GroupByBox.BandLabelAppearance = appearance17;
            this.cmbDocType.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance18.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance18.BackColor2 = System.Drawing.SystemColors.Control;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbDocType.DisplayLayout.GroupByBox.PromptAppearance = appearance18;
            this.cmbDocType.DisplayLayout.MaxColScrollRegions = 1;
            this.cmbDocType.DisplayLayout.MaxRowScrollRegions = 1;
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            appearance19.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbDocType.DisplayLayout.Override.ActiveCellAppearance = appearance19;
            appearance20.BackColor = System.Drawing.SystemColors.Highlight;
            appearance20.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.cmbDocType.DisplayLayout.Override.ActiveRowAppearance = appearance20;
            this.cmbDocType.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.cmbDocType.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.cmbDocType.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            this.cmbDocType.DisplayLayout.Override.CardAreaAppearance = appearance21;
            appearance22.BorderColor = System.Drawing.Color.Silver;
            appearance22.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.cmbDocType.DisplayLayout.Override.CellAppearance = appearance22;
            this.cmbDocType.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.cmbDocType.DisplayLayout.Override.CellPadding = 0;
            appearance23.BackColor = System.Drawing.SystemColors.Control;
            appearance23.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance23.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance23.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance23.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbDocType.DisplayLayout.Override.GroupByRowAppearance = appearance23;
            appearance24.TextHAlignAsString = "Left";
            this.cmbDocType.DisplayLayout.Override.HeaderAppearance = appearance24;
            this.cmbDocType.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.cmbDocType.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            appearance25.BorderColor = System.Drawing.Color.Silver;
            this.cmbDocType.DisplayLayout.Override.RowAppearance = appearance25;
            this.cmbDocType.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance26.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cmbDocType.DisplayLayout.Override.TemplateAddRowAppearance = appearance26;
            this.cmbDocType.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.cmbDocType.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.cmbDocType.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.cmbDocType.Location = new System.Drawing.Point(122, 106);
            this.cmbDocType.Name = "cmbDocType";
            this.cmbDocType.Size = new System.Drawing.Size(371, 23);
            this.cmbDocType.TabIndex = 59;
            // 
            // cmbItem
            // 
            appearance27.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbItem.Appearance = appearance27;
            this.cmbItem.CheckedListSettings.CheckStateMember = "";
            appearance28.BackColor = System.Drawing.SystemColors.Window;
            appearance28.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbItem.DisplayLayout.Appearance = appearance28;
            this.cmbItem.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.cmbItem.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance29.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance29.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance29.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance29.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.GroupByBox.Appearance = appearance29;
            appearance30.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbItem.DisplayLayout.GroupByBox.BandLabelAppearance = appearance30;
            this.cmbItem.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance31.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance31.BackColor2 = System.Drawing.SystemColors.Control;
            appearance31.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance31.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbItem.DisplayLayout.GroupByBox.PromptAppearance = appearance31;
            this.cmbItem.DisplayLayout.MaxColScrollRegions = 1;
            this.cmbItem.DisplayLayout.MaxRowScrollRegions = 1;
            appearance32.BackColor = System.Drawing.SystemColors.Window;
            appearance32.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbItem.DisplayLayout.Override.ActiveCellAppearance = appearance32;
            appearance33.BackColor = System.Drawing.SystemColors.Highlight;
            appearance33.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.cmbItem.DisplayLayout.Override.ActiveRowAppearance = appearance33;
            this.cmbItem.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.cmbItem.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.cmbItem.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance34.BackColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.Override.CardAreaAppearance = appearance34;
            appearance35.BorderColor = System.Drawing.Color.Silver;
            appearance35.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.cmbItem.DisplayLayout.Override.CellAppearance = appearance35;
            this.cmbItem.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.cmbItem.DisplayLayout.Override.CellPadding = 0;
            appearance36.BackColor = System.Drawing.SystemColors.Control;
            appearance36.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance36.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance36.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance36.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.Override.GroupByRowAppearance = appearance36;
            appearance37.TextHAlignAsString = "Left";
            this.cmbItem.DisplayLayout.Override.HeaderAppearance = appearance37;
            this.cmbItem.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.cmbItem.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance38.BackColor = System.Drawing.SystemColors.Window;
            appearance38.BorderColor = System.Drawing.Color.Silver;
            this.cmbItem.DisplayLayout.Override.RowAppearance = appearance38;
            this.cmbItem.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance39.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cmbItem.DisplayLayout.Override.TemplateAddRowAppearance = appearance39;
            this.cmbItem.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.cmbItem.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.cmbItem.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.cmbItem.Location = new System.Drawing.Point(122, 73);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(371, 23);
            this.cmbItem.TabIndex = 58;
            // 
            // ChkTranType
            // 
            this.ChkTranType.AutoSize = true;
            this.ChkTranType.Location = new System.Drawing.Point(16, 106);
            this.ChkTranType.Name = "ChkTranType";
            this.ChkTranType.Size = new System.Drawing.Size(80, 17);
            this.ChkTranType.TabIndex = 5;
            this.ChkTranType.Text = "Doc Type";
            this.ChkTranType.UseVisualStyleBackColor = true;
            this.ChkTranType.CheckedChanged += new System.EventHandler(this.ChkTranType_CheckedChanged);
            // 
            // CheckItemID
            // 
            this.CheckItemID.AutoSize = true;
            this.CheckItemID.Location = new System.Drawing.Point(16, 73);
            this.CheckItemID.Name = "CheckItemID";
            this.CheckItemID.Size = new System.Drawing.Size(71, 17);
            this.CheckItemID.TabIndex = 4;
            this.CheckItemID.Text = "Item ID";
            this.CheckItemID.UseVisualStyleBackColor = true;
            this.CheckItemID.CheckedChanged += new System.EventHandler(this.CheckItemID_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Location = new System.Drawing.Point(438, 164);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(55, 32);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.Color.Transparent;
            this.btnView.Location = new System.Drawing.Point(346, 164);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(86, 32);
            this.btnView.TabIndex = 3;
            this.btnView.Text = "View Report";
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(370, 46);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(123, 21);
            this.dtpToDate.TabIndex = 0;
            // 
            // rbtAll
            // 
            this.rbtAll.AutoSize = true;
            this.rbtAll.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtAll.Location = new System.Drawing.Point(16, 21);
            this.rbtAll.Name = "rbtAll";
            this.rbtAll.Size = new System.Drawing.Size(38, 17);
            this.rbtAll.TabIndex = 2;
            this.rbtAll.TabStop = true;
            this.rbtAll.Text = "All";
            this.rbtAll.UseVisualStyleBackColor = true;
            this.rbtAll.CheckedChanged += new System.EventHandler(this.rbtAll_CheckedChanged);
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(191, 47);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(121, 21);
            this.dtpFromDate.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "From Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(318, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "To Date";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnView);
            this.groupBox1.Controls.Add(this.ChkWarehouse);
            this.groupBox1.Controls.Add(this.ChkTranType);
            this.groupBox1.Controls.Add(this.cmbWarehouse);
            this.groupBox1.Controls.Add(this.CheckItemID);
            this.groupBox1.Controls.Add(this.chkDateRange);
            this.groupBox1.Controls.Add(this.rbtAll);
            this.groupBox1.Controls.Add(this.cmbDocType);
            this.groupBox1.Controls.Add(this.dtpToDate);
            this.groupBox1.Controls.Add(this.cmbItem);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpFromDate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 200);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // frmInventoryMovement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(530, 211);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmInventoryMovement";
            this.Text = "Inventory  Movement";
            this.Load += new System.EventHandler(this.frmInventoryMovement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cmbWarehouse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDocType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItem)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.RadioButton rbtAll;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ChkTranType;
        private System.Windows.Forms.CheckBox CheckItemID;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbItem;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbDocType;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbWarehouse;
        private System.Windows.Forms.CheckBox ChkWarehouse;
        private System.Windows.Forms.CheckBox chkDateRange;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
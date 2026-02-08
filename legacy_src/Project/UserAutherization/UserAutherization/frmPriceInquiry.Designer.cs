namespace UserAutherization
{
    partial class frmPriceInquiry
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cmbItem = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.dgvDeliveryNoteList = new System.Windows.Forms.DataGridView();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DDeliveryNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Customer_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Deafault = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.cmbItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeliveryNoteList)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.Location = new System.Drawing.Point(12, 15);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(45, 23);
            this.ultraLabel1.TabIndex = 62;
            this.ultraLabel1.Text = "ItemID";
            // 
            // cmbItem
            // 
            appearance1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbItem.Appearance = appearance1;
            this.cmbItem.CheckedListSettings.CheckStateMember = "";
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbItem.DisplayLayout.Appearance = appearance2;
            this.cmbItem.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.cmbItem.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbItem.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.cmbItem.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cmbItem.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.cmbItem.DisplayLayout.MaxColScrollRegions = 1;
            this.cmbItem.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbItem.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.cmbItem.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.cmbItem.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.cmbItem.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.cmbItem.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.cmbItem.DisplayLayout.Override.CellAppearance = appearance9;
            this.cmbItem.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.cmbItem.DisplayLayout.Override.CellPadding = 0;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.BorderColor = System.Drawing.SystemColors.Window;
            this.cmbItem.DisplayLayout.Override.GroupByRowAppearance = appearance10;
            appearance11.TextHAlignAsString = "Left";
            this.cmbItem.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.cmbItem.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.cmbItem.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            this.cmbItem.DisplayLayout.Override.RowAppearance = appearance12;
            this.cmbItem.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cmbItem.DisplayLayout.Override.TemplateAddRowAppearance = appearance13;
            this.cmbItem.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.cmbItem.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.cmbItem.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.cmbItem.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbItem.Location = new System.Drawing.Point(69, 12);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(272, 25);
            this.cmbItem.TabIndex = 61;
            this.cmbItem.RowSelected += new Infragistics.Win.UltraWinGrid.RowSelectedEventHandler(this.cmbItem_RowSelected);
            // 
            // dgvDeliveryNoteList
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvDeliveryNoteList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDeliveryNoteList.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDeliveryNoteList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDeliveryNoteList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDeliveryNoteList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.DDeliveryNo,
            this.Customer_ID,
            this.Date,
            this.Deafault});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDeliveryNoteList.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDeliveryNoteList.Location = new System.Drawing.Point(12, 49);
            this.dgvDeliveryNoteList.Name = "dgvDeliveryNoteList";
            this.dgvDeliveryNoteList.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dgvDeliveryNoteList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDeliveryNoteList.Size = new System.Drawing.Size(434, 268);
            this.dgvDeliveryNoteList.TabIndex = 63;
            // 
            // No
            // 
            this.No.HeaderText = "Index";
            this.No.Name = "No";
            this.No.Width = 40;
            // 
            // DDeliveryNo
            // 
            this.DDeliveryNo.HeaderText = "Min Qty";
            this.DDeliveryNo.Name = "DDeliveryNo";
            this.DDeliveryNo.Width = 80;
            // 
            // Customer_ID
            // 
            this.Customer_ID.HeaderText = "Max Qty";
            this.Customer_ID.Name = "Customer_ID";
            this.Customer_ID.Width = 80;
            // 
            // Date
            // 
            this.Date.HeaderText = "Price";
            this.Date.Name = "Date";
            this.Date.Width = 125;
            // 
            // Deafault
            // 
            this.Deafault.HeaderText = "Default";
            this.Deafault.Name = "Deafault";
            // 
            // frmPriceInquiry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(458, 319);
            this.Controls.Add(this.dgvDeliveryNoteList);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.cmbItem);
            this.Name = "frmPriceInquiry";
            this.Text = "Price Inquiry";
            this.Load += new System.EventHandler(this.frmPriceInquiry_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cmbItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeliveryNoteList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbItem;
        private System.Windows.Forms.DataGridView dgvDeliveryNoteList;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn DDeliveryNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Customer_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Deafault;
    }
}
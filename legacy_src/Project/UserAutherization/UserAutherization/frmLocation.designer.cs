namespace UserAutherization
{
    partial class frmLocation
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLocation));
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.Phase = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ucmbLocCode = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnFind = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label35 = new System.Windows.Forms.Label();
            this.erpMaster = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucmbLocCode)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.erpMaster)).BeginInit();
            this.SuspendLayout();
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(271, 47);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(72, 17);
            this.chkActive.TabIndex = 0;
            this.chkActive.Text = "Inactive";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // Phase
            // 
            this.Phase.AutoSize = true;
            this.Phase.Location = new System.Drawing.Point(31, 48);
            this.Phase.Name = "Phase";
            this.Phase.Size = new System.Drawing.Size(72, 13);
            this.Phase.TabIndex = 1;
            this.Phase.Text = "Location ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ucmbLocCode);
            this.panel1.Controls.Add(this.txtDescription);
            this.panel1.Controls.Add(this.chkActive);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.Phase);
            this.panel1.Location = new System.Drawing.Point(12, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(394, 183);
            this.panel1.TabIndex = 3;
            // 
            // ucmbLocCode
            // 
            this.ucmbLocCode.CheckedListSettings.CheckStateMember = "";
            appearance13.BackColor = System.Drawing.SystemColors.ControlLight;
            appearance13.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.ucmbLocCode.DisplayLayout.Appearance = appearance13;
            this.ucmbLocCode.DisplayLayout.InterBandSpacing = 10;
            this.ucmbLocCode.DisplayLayout.MaxColScrollRegions = 1;
            this.ucmbLocCode.DisplayLayout.MaxRowScrollRegions = 1;
            this.ucmbLocCode.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            appearance14.BackColor = System.Drawing.Color.Transparent;
            this.ucmbLocCode.DisplayLayout.Override.CardAreaAppearance = appearance14;
            appearance15.BackColor = System.Drawing.SystemColors.Control;
            appearance15.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.ucmbLocCode.DisplayLayout.Override.CellAppearance = appearance15;
            this.ucmbLocCode.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ucmbLocCode.DisplayLayout.Override.FilterEvaluationTrigger = Infragistics.Win.UltraWinGrid.FilterEvaluationTrigger.OnCellValueChange;
            this.ucmbLocCode.DisplayLayout.Override.FilterOperandDropDownItems = ((Infragistics.Win.UltraWinGrid.FilterOperandDropDownItems)(((((((Infragistics.Win.UltraWinGrid.FilterOperandDropDownItems.Blanks | Infragistics.Win.UltraWinGrid.FilterOperandDropDownItems.NonBlanks)
                        | Infragistics.Win.UltraWinGrid.FilterOperandDropDownItems.Errors)
                        | Infragistics.Win.UltraWinGrid.FilterOperandDropDownItems.NonErrors)
                        | Infragistics.Win.UltraWinGrid.FilterOperandDropDownItems.Custom)
                        | Infragistics.Win.UltraWinGrid.FilterOperandDropDownItems.CellValues)
                        | Infragistics.Win.UltraWinGrid.FilterOperandDropDownItems.All)));
            this.ucmbLocCode.DisplayLayout.Override.FixedRowSortOrder = Infragistics.Win.UltraWinGrid.FixedRowSortOrder.Sorted;
            appearance16.BackColor = System.Drawing.SystemColors.Control;
            appearance16.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance16.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.ucmbLocCode.DisplayLayout.Override.HeaderAppearance = appearance16;
            this.ucmbLocCode.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance17.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.ucmbLocCode.DisplayLayout.Override.RowSelectorAppearance = appearance17;
            appearance18.BackColor = System.Drawing.SystemColors.InactiveCaption;
            appearance18.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.ucmbLocCode.DisplayLayout.Override.SelectedRowAppearance = appearance18;
            this.ucmbLocCode.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ucmbLocCode.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.ucmbLocCode.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ucmbLocCode.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ucmbLocCode.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ucmbLocCode.DropDownSearchMethod = Infragistics.Win.UltraWinGrid.DropDownSearchMethod.Linear;
            this.ucmbLocCode.Location = new System.Drawing.Point(108, 44);
            this.ucmbLocCode.Name = "ucmbLocCode";
            this.ucmbLocCode.Size = new System.Drawing.Size(157, 23);
            this.ucmbLocCode.TabIndex = 0;
            this.ucmbLocCode.RowSelected += new Infragistics.Win.UltraWinGrid.RowSelectedEventHandler(this.ucmbLocCode_RowSelected);
            this.ucmbLocCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ucmbLocCode_KeyDown);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(108, 71);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(235, 21);
            this.txtDescription.TabIndex = 1;
            this.txtDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDescription_KeyDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClear,
            this.btnSave,
            this.btnFind,
            this.btnDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(419, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnClear
            // 
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(54, 22);
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
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
            // btnFind
            // 
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(45, 22);
            this.btnFind.Text = "List";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(60, 22);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "b_firstpage.png");
            this.imageList1.Images.SetKeyName(1, "b_lastpage.png");
            this.imageList1.Images.SetKeyName(2, "b_nextpage.png");
            this.imageList1.Images.SetKeyName(3, "b_prevpage.png");
            this.imageList1.Images.SetKeyName(4, "clear.jpg");
            this.imageList1.Images.SetKeyName(5, "CostCodes.png");
            this.imageList1.Images.SetKeyName(6, "fileclip.jpeg");
            this.imageList1.Images.SetKeyName(7, "listNEw.jpg");
            this.imageList1.Images.SetKeyName(8, "Notea.jpg");
            this.imageList1.Images.SetKeyName(9, "popup.gif");
            this.imageList1.Images.SetKeyName(10, "print.jpg");
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(320, 31);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(92, 23);
            this.label35.TabIndex = 140;
            this.label35.Text = "Locations";
            // 
            // erpMaster
            // 
            this.erpMaster.ContainerControl = this;
            // 
            // frmLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(419, 253);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmLocation";
            this.Text = "Locations";
            this.Load += new System.EventHandler(this.frmLocation_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucmbLocCode)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.erpMaster)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Label Phase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnFind;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox txtDescription;
        private Infragistics.Win.UltraWinGrid.UltraCombo ucmbLocCode;
        private System.Windows.Forms.ErrorProvider erpMaster;
    }
}
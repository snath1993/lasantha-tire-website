namespace UserAutherization
{
    partial class frmPrintTaxInvoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrintTaxInvoice));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.crvTaxInvoice = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.axAcroPDF1 = new AxAcroPDFLib.AxAcroPDF();
            this.axAcroPDF2 = new AxAcroPDFLib.AxAcroPDF();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF2)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // crvTaxInvoice
            // 
            this.crvTaxInvoice.ActiveViewIndex = -1;
            this.crvTaxInvoice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvTaxInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvTaxInvoice.Location = new System.Drawing.Point(0, 0);
            this.crvTaxInvoice.Name = "crvTaxInvoice";
            this.crvTaxInvoice.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.crvTaxInvoice.SelectionFormula = "";
            this.crvTaxInvoice.Size = new System.Drawing.Size(705, 508);
            this.crvTaxInvoice.TabIndex = 0;
            this.crvTaxInvoice.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            this.crvTaxInvoice.ToolPanelWidth = 0;
            this.crvTaxInvoice.ViewTimeSelectionFormula = "";
            // 
            // axAcroPDF1
            // 
            this.axAcroPDF1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAcroPDF1.Enabled = true;
            this.axAcroPDF1.Location = new System.Drawing.Point(0, 0);
            this.axAcroPDF1.Name = "axAcroPDF1";
            this.axAcroPDF1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAcroPDF1.OcxState")));
            this.axAcroPDF1.Size = new System.Drawing.Size(676, 508);
            this.axAcroPDF1.TabIndex = 1;
            this.axAcroPDF1.Visible = false;
            // 
            // axAcroPDF2
            // 
            this.axAcroPDF2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAcroPDF2.Enabled = true;
            this.axAcroPDF2.Location = new System.Drawing.Point(0, 35);
            this.axAcroPDF2.Name = "axAcroPDF2";
            this.axAcroPDF2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAcroPDF2.OcxState")));
            this.axAcroPDF2.Size = new System.Drawing.Size(676, 473);
            this.axAcroPDF2.TabIndex = 81;
            this.axAcroPDF2.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = global::UserAutherization.Properties.Resources.BBar;
            this.toolStrip1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClose,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(705, 27);
            this.toolStrip1.TabIndex = 93;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnClose
            // 
            this.btnClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnClose.Image = global::UserAutherization.Properties.Resources.close1;
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(63, 24);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_3);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.Image = global::UserAutherization.Properties.Resources.multi_edit;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(121, 24);
            this.toolStripButton1.Text = "Go to Edit Mode";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // frmPrintTaxInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 508);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.crvTaxInvoice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPrintTaxInvoice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tax Invoice";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmPrintTaxInvoice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF2)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvTaxInvoice;
        private AxAcroPDFLib.AxAcroPDF axAcroPDF1;
        private AxAcroPDFLib.AxAcroPDF axAcroPDF2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}
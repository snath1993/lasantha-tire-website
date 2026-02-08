namespace UserAutherization
{
    partial class frmScanReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScanReport));
            this.crvScanReport = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvScanReport
            // 
            this.crvScanReport.ActiveViewIndex = -1;
            this.crvScanReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvScanReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvScanReport.Location = new System.Drawing.Point(0, 0);
            this.crvScanReport.Name = "crvScanReport";
            this.crvScanReport.SelectionFormula = "";
            this.crvScanReport.Size = new System.Drawing.Size(631, 515);
            this.crvScanReport.TabIndex = 0;
            this.crvScanReport.ViewTimeSelectionFormula = "";
            // 
            // frmScanReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 515);
            this.Controls.Add(this.crvScanReport);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmScanReport";
            this.Text = "ScanReport";
            this.Load += new System.EventHandler(this.frmScanReport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvScanReport;
    }
}
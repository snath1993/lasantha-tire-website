namespace UserAutherization
{
    partial class XrayReport
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
            this.crvXrayReport = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvXrayReport
            // 
            this.crvXrayReport.ActiveViewIndex = -1;
            this.crvXrayReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvXrayReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvXrayReport.Location = new System.Drawing.Point(0, 0);
            this.crvXrayReport.Name = "crvXrayReport";
            this.crvXrayReport.SelectionFormula = "";
            this.crvXrayReport.Size = new System.Drawing.Size(631, 458);
            this.crvXrayReport.TabIndex = 0;
            this.crvXrayReport.ViewTimeSelectionFormula = "";
            // 
            // XrayReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 458);
            this.Controls.Add(this.crvXrayReport);
            this.Name = "XrayReport";
            this.Text = "XrayReport";
            this.Load += new System.EventHandler(this.XrayReport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvXrayReport;
    }
}
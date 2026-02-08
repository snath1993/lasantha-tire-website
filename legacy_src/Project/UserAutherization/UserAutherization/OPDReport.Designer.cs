namespace UserAutherization
{
    partial class OPDReport
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
            this.crvopdReport = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvopdReport
            // 
            this.crvopdReport.ActiveViewIndex = -1;
            this.crvopdReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvopdReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvopdReport.Location = new System.Drawing.Point(0, 0);
            this.crvopdReport.Name = "crvopdReport";
            this.crvopdReport.SelectionFormula = "";
            this.crvopdReport.Size = new System.Drawing.Size(641, 555);
            this.crvopdReport.TabIndex = 0;
            this.crvopdReport.ViewTimeSelectionFormula = "";
            // 
            // OPDReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 555);
            this.Controls.Add(this.crvopdReport);
            this.Name = "OPDReport";
            this.Text = "OPDReport";
            this.Load += new System.EventHandler(this.OPDReport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvopdReport;
    }
}
namespace UserAutherization
{
    partial class frmPOrVariicationPrint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPOrVariicationPrint));
            this.crvPOVarisication = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvPOVarisication
            // 
            this.crvPOVarisication.ActiveViewIndex = -1;
            this.crvPOVarisication.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvPOVarisication.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvPOVarisication.Location = new System.Drawing.Point(0, 0);
            this.crvPOVarisication.Name = "crvPOVarisication";
            this.crvPOVarisication.SelectionFormula = "";
            this.crvPOVarisication.Size = new System.Drawing.Size(822, 580);
            this.crvPOVarisication.TabIndex = 0;
            this.crvPOVarisication.ViewTimeSelectionFormula = "";
            // 
            // frmPOrVariicationPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 580);
            this.Controls.Add(this.crvPOVarisication);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPOrVariicationPrint";
            this.Text = "PO Status ";
            this.Load += new System.EventHandler(this.frmPOrVariicationPrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvPOVarisication;
    }
}
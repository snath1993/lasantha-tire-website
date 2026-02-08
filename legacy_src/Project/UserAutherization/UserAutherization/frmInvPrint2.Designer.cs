namespace UserAutherization
{
    partial class frmInvPrint2
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
            this.crvTaxInvoice = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvTaxInvoice
            // 
            this.crvTaxInvoice.ActiveViewIndex = -1;
            this.crvTaxInvoice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvTaxInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvTaxInvoice.Location = new System.Drawing.Point(0, 0);
            this.crvTaxInvoice.Name = "crvTaxInvoice";
            this.crvTaxInvoice.SelectionFormula = "";
            this.crvTaxInvoice.Size = new System.Drawing.Size(292, 266);
            this.crvTaxInvoice.TabIndex = 1;
            this.crvTaxInvoice.ViewTimeSelectionFormula = "";
            // 
            // frmInvPrint2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.crvTaxInvoice);
            this.Name = "frmInvPrint2";
            this.Text = "frmInvPrint2";
            this.Load += new System.EventHandler(this.frmInvPrint2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvTaxInvoice;
    }
}
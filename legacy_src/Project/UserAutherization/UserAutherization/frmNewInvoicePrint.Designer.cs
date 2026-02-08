namespace UserAutherization
{
    partial class frmNewInvoicePrint
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
            this.CrvNewInvoice = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CrvNewInvoice
            // 
            this.CrvNewInvoice.ActiveViewIndex = -1;
            this.CrvNewInvoice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CrvNewInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrvNewInvoice.Location = new System.Drawing.Point(0, 0);
            this.CrvNewInvoice.Name = "CrvNewInvoice";
            this.CrvNewInvoice.SelectionFormula = "";
            this.CrvNewInvoice.Size = new System.Drawing.Size(684, 546);
            this.CrvNewInvoice.TabIndex = 0;
            this.CrvNewInvoice.ViewTimeSelectionFormula = "";
            // 
            // frmNewInvoicePrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 546);
            this.Controls.Add(this.CrvNewInvoice);
            this.Name = "frmNewInvoicePrint";
            this.Text = "frmNewInvoicePrint";
            this.Load += new System.EventHandler(this.frmNewInvoicePrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CrvNewInvoice;
    }
}
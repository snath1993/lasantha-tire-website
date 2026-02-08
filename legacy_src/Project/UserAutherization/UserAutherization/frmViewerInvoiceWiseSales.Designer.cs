namespace UserAutherization
{
    partial class frmViewerInvoiceWiseSales
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
            this.crvInvoiceWiseSales = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvInvoiceWiseSales
            // 
            this.crvInvoiceWiseSales.ActiveViewIndex = -1;
            this.crvInvoiceWiseSales.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvInvoiceWiseSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvInvoiceWiseSales.Location = new System.Drawing.Point(0, 0);
            this.crvInvoiceWiseSales.Name = "crvInvoiceWiseSales";
            this.crvInvoiceWiseSales.SelectionFormula = "";
            this.crvInvoiceWiseSales.Size = new System.Drawing.Size(308, 326);
            this.crvInvoiceWiseSales.TabIndex = 0;
            this.crvInvoiceWiseSales.ViewTimeSelectionFormula = "";
            // 
            // frmViewerInvoiceWiseSales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 326);
            this.Controls.Add(this.crvInvoiceWiseSales);
            this.Name = "frmViewerInvoiceWiseSales";
            this.Text = " Invoice Wise Sales";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmViewerInvoiceWiseSales_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvInvoiceWiseSales;
    }
}
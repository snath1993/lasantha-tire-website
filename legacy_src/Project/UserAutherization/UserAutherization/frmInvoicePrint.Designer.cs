namespace UserAutherization
{
    partial class frmInvoicePrint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInvoicePrint));
            this.crvInvoicePrint = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvInvoicePrint
            // 
            this.crvInvoicePrint.ActiveViewIndex = -1;
            this.crvInvoicePrint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvInvoicePrint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvInvoicePrint.Location = new System.Drawing.Point(0, 0);
            this.crvInvoicePrint.Name = "crvInvoicePrint";
            this.crvInvoicePrint.SelectionFormula = "";
            this.crvInvoicePrint.Size = new System.Drawing.Size(663, 531);
            this.crvInvoicePrint.TabIndex = 0;
            this.crvInvoicePrint.ViewTimeSelectionFormula = "";
            this.crvInvoicePrint.Load += new System.EventHandler(this.crvInvoicePrint_Load);
            // 
            // frmInvoicePrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 531);
            this.Controls.Add(this.crvInvoicePrint);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmInvoicePrint";
            this.Text = "Customer Invoice";
            this.Load += new System.EventHandler(this.frmInvoicePrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvInvoicePrint;
    }
}
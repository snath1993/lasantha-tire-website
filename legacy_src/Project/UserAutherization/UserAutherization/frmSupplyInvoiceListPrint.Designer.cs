namespace UserAutherization
{
    partial class frmSupplyInvoiceListPrint
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
            this.crvSupplyInvList = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvSupplyInvList
            // 
            this.crvSupplyInvList.ActiveViewIndex = -1;
            this.crvSupplyInvList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvSupplyInvList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvSupplyInvList.Location = new System.Drawing.Point(0, 0);
            this.crvSupplyInvList.Name = "crvSupplyInvList";
            this.crvSupplyInvList.SelectionFormula = "";
            this.crvSupplyInvList.Size = new System.Drawing.Size(729, 577);
            this.crvSupplyInvList.TabIndex = 0;
            this.crvSupplyInvList.ViewTimeSelectionFormula = "";
            // 
            // frmSupplyInvoiceListPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 577);
            this.Controls.Add(this.crvSupplyInvList);
            this.Name = "frmSupplyInvoiceListPrint";
            this.Text = "Supplier Invoices";
            this.Load += new System.EventHandler(this.frmSupplyInvoiceListPrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer crvSupplyInvList;
    }
}
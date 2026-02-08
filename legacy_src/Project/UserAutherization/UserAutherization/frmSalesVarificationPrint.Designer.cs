namespace UserAutherization
{
    partial class frmSalesVarificationPrint
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
            this.CRVSalesVarification = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // CRVSalesVarification
            // 
            this.CRVSalesVarification.ActiveViewIndex = -1;
            this.CRVSalesVarification.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRVSalesVarification.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRVSalesVarification.Location = new System.Drawing.Point(0, 0);
            this.CRVSalesVarification.Name = "CRVSalesVarification";
            this.CRVSalesVarification.SelectionFormula = "";
            this.CRVSalesVarification.Size = new System.Drawing.Size(629, 519);
            this.CRVSalesVarification.TabIndex = 0;
            this.CRVSalesVarification.ViewTimeSelectionFormula = "";
            // 
            // frmSalesVarificationPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 519);
            this.Controls.Add(this.CRVSalesVarification);
            this.Name = "frmSalesVarificationPrint";
            this.Text = "frmSalesVarificationPrint";
            this.Load += new System.EventHandler(this.frmSalesVarificationPrint_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRVSalesVarification;
    }
}